from __future__ import annotations

from dataclasses import dataclass
from datetime import datetime, timezone
from pathlib import Path
from xml.sax.saxutils import escape
from zipfile import ZIP_DEFLATED, ZipFile


SLIDE_WIDTH = 12192000
SLIDE_HEIGHT = 6858000

BG = "050505"
GOLD = "D4AF37"
LIGHT = "E5E0DF"
WHITE = "FFFFFF"
GRAY = "3B3B3B"

TITLE_FONT = "Montserrat Black"
BODY_FONT = "Montserrat Bold"
ACCENT_FONT = "Syne"


def emu(inches: float) -> int:
    return int(inches * 914400)


def xml(text: str) -> str:
    return '<?xml version="1.0" encoding="UTF-8" standalone="yes"?>\n' + text.strip()


def group_shape_base() -> str:
    return """
    <p:nvGrpSpPr>
      <p:cNvPr id="1" name=""/>
      <p:cNvGrpSpPr/>
      <p:nvPr/>
    </p:nvGrpSpPr>
    <p:grpSpPr>
      <a:xfrm>
        <a:off x="0" y="0"/>
        <a:ext cx="0" cy="0"/>
        <a:chOff x="0" y="0"/>
        <a:chExt cx="0" cy="0"/>
      </a:xfrm>
    </p:grpSpPr>
    """.strip()


def fill_xml(color: str | None) -> str:
    if not color:
        return "<a:noFill/>"
    return f"<a:solidFill><a:srgbClr val=\"{color}\"/></a:solidFill>"


def line_xml(color: str | None, width: int = 12700) -> str:
    if not color:
        return "<a:ln><a:noFill/></a:ln>"
    return (
        f'<a:ln w="{width}">'
        f"<a:solidFill><a:srgbClr val=\"{color}\"/></a:solidFill>"
        "</a:ln>"
    )


def shape_xml(
    shape_id: int,
    name: str,
    x: int,
    y: int,
    cx: int,
    cy: int,
    *,
    fill: str | None = None,
    line: str | None = None,
    preset: str = "rect",
) -> str:
    return f"""
    <p:sp>
      <p:nvSpPr>
        <p:cNvPr id="{shape_id}" name="{escape(name)}"/>
        <p:cNvSpPr/>
        <p:nvPr/>
      </p:nvSpPr>
      <p:spPr>
        <a:xfrm>
          <a:off x="{x}" y="{y}"/>
          <a:ext cx="{cx}" cy="{cy}"/>
        </a:xfrm>
        <a:prstGeom prst="{preset}">
          <a:avLst/>
        </a:prstGeom>
        {fill_xml(fill)}
        {line_xml(line)}
      </p:spPr>
    </p:sp>
    """.strip()


def paragraph_xml(
    text: str,
    *,
    size: int,
    color: str,
    font: str,
    bold: bool = False,
    bullet: bool = False,
    align: str | None = None,
) -> str:
    attrs = [f'sz="{size}"', f'lang="pt-BR"']
    if bold:
        attrs.append('b="1"')

    ppr_parts: list[str] = []
    if bullet:
        ppr_parts.append('marL="342900"')
        ppr_parts.append('indent="-285750"')
    if align:
        ppr_parts.append(f'algn="{align}"')
    ppr_attr = (" " + " ".join(ppr_parts)) if ppr_parts else ""

    bullet_xml = '<a:buChar char="•"/>' if bullet else "<a:buNone/>"

    return (
        "<a:p>"
        f"<a:pPr{ppr_attr}>{bullet_xml}</a:pPr>"
        f"<a:r><a:rPr {' '.join(attrs)}>"
        f"<a:solidFill><a:srgbClr val=\"{color}\"/></a:solidFill>"
        f"<a:latin typeface=\"{escape(font)}\"/>"
        "</a:rPr>"
        f"<a:t>{escape(text)}</a:t></a:r>"
        f"<a:endParaRPr lang=\"pt-BR\" sz=\"{size}\"/>"
        "</a:p>"
    )


def text_shape_xml(
    shape_id: int,
    name: str,
    x: int,
    y: int,
    cx: int,
    cy: int,
    paragraphs: list[str],
    *,
    fill: str | None = None,
    line: str | None = None,
    preset: str = "rect",
    anchor: str = "t",
) -> str:
    return f"""
    <p:sp>
      <p:nvSpPr>
        <p:cNvPr id="{shape_id}" name="{escape(name)}"/>
        <p:cNvSpPr txBox="1"/>
        <p:nvPr/>
      </p:nvSpPr>
      <p:spPr>
        <a:xfrm>
          <a:off x="{x}" y="{y}"/>
          <a:ext cx="{cx}" cy="{cy}"/>
        </a:xfrm>
        <a:prstGeom prst="{preset}">
          <a:avLst/>
        </a:prstGeom>
        {fill_xml(fill)}
        {line_xml(line)}
      </p:spPr>
      <p:txBody>
        <a:bodyPr wrap="square" anchor="{anchor}" lIns="91440" tIns="45720" rIns="91440" bIns="45720">
          <a:spAutoFit/>
        </a:bodyPr>
        <a:lstStyle/>
        {''.join(paragraphs)}
      </p:txBody>
    </p:sp>
    """.strip()


def picture_xml(
    shape_id: int,
    name: str,
    x: int,
    y: int,
    cx: int,
    cy: int,
    rel_id: str,
) -> str:
    return f"""
    <p:pic>
      <p:nvPicPr>
        <p:cNvPr id="{shape_id}" name="{escape(name)}"/>
        <p:cNvPicPr>
          <a:picLocks noChangeAspect="1"/>
        </p:cNvPicPr>
        <p:nvPr/>
      </p:nvPicPr>
      <p:blipFill>
        <a:blip r:embed="{rel_id}"/>
        <a:stretch><a:fillRect/></a:stretch>
      </p:blipFill>
      <p:spPr>
        <a:xfrm>
          <a:off x="{x}" y="{y}"/>
          <a:ext cx="{cx}" cy="{cy}"/>
        </a:xfrm>
        <a:prstGeom prst="rect">
          <a:avLst/>
        </a:prstGeom>
        <a:ln><a:noFill/></a:ln>
      </p:spPr>
    </p:pic>
    """.strip()


@dataclass
class SlideSpec:
    section: str
    title: str
    bullets: list[str]
    note_title: str
    note_lines: list[str]


SLIDES = [
    SlideSpec(
        section="VISAO GERAL",
        title="OmniMarket, em resumo",
        bullets=[
            "Marketplace P2P com foco em pequenos vendedores.",
            "Fluxo centralizado de cadastro, produtos, pedidos e pagamento.",
            "Projeto de TCC com base tecnica aplicavel a um cenario real.",
        ],
        note_title="Proposta",
        note_lines=[
            "Conectar compradores e vendedores em uma unica plataforma.",
            "Transformar o TCC em uma solucao coerente, clara e demonstravel.",
        ],
    ),
    SlideSpec(
        section="ANDAMENTO",
        title="Onde estamos hoje",
        bullets=[
            "API em .NET 9 com Entity Framework Core e SQL Server.",
            "Autenticacao com JWT e controle de acesso por perfil.",
            "Painel admin ja funcional para visao operacional.",
            "Web e Mobile ainda em evolucao estrutural.",
        ],
        note_title="Leitura rapida",
        note_lines=[
            "A parte mais madura do projeto esta no back-end.",
            "O foco agora e equilibrar regra de negocio e experiencia visual.",
        ],
    ),
    SlideSpec(
        section="SPRINT ATUAL",
        title="Destaque da sprint",
        bullets=[
            "Plano de pagamento por pedido.",
            "Separacao financeira por vendedor no mesmo fluxo.",
            "Comissao, repasse, recibo e historico financeiro.",
            "Webhook e conciliacao prontos para evolucao futura.",
        ],
        note_title="Modulo financeiro",
        note_lines=[
            "Foi o maior salto de maturidade desta etapa.",
            "A modelagem ficou mais proxima do funcionamento real de um marketplace.",
        ],
    ),
    SlideSpec(
        section="VALIDACAO",
        title="Como validamos a entrega",
        bullets=[
            "Endpoints financeiros disponiveis no Swagger.",
            "Collections Postman com cenarios prontos.",
            "Gateway fake para confirmar e conciliar pagamentos.",
            "Teste com Pix, cartao, pagamento misto e dois vendedores.",
        ],
        note_title="Cenarios",
        note_lines=[
            "1 vendedor e 1 forma de pagamento.",
            "2 vendedores no mesmo pedido.",
            "Pix, cartao e pagamento parcelado.",
        ],
    ),
    SlideSpec(
        section="FECHAMENTO",
        title="Proximos passos",
        bullets=[
            "Evoluir as interfaces Web e Mobile.",
            "Refinar testes, documentacao e demonstracao final.",
            "Aproximar o financeiro de uma integracao real.",
        ],
        note_title="Sintese da sprint",
        note_lines=[
            "A base tecnica ficou mais robusta.",
            "O proximo foco sera acabamento, front-end e validacao final do TCC.",
        ],
    ),
]


def build_title_slide() -> str:
    shapes: list[str] = []
    shape_id = 2

    shapes.append(shape_xml(shape_id, "Background", 0, 0, SLIDE_WIDTH, SLIDE_HEIGHT, fill=BG, line=None))
    shape_id += 1
    shapes.append(shape_xml(shape_id, "Top Line", emu(0.85), emu(0.55), emu(11.6), emu(0.05), fill=GOLD, line=None))
    shape_id += 1
    shapes.append(shape_xml(shape_id, "Bottom Line", emu(0.85), emu(6.05), emu(11.6), emu(0.05), fill=GOLD, line=None))
    shape_id += 1

    shapes.append(
        picture_xml(
            shape_id,
            "Logo",
            emu(4.95),
            emu(0.95),
            emu(3.4),
            emu(3.4),
            "rId2",
        )
    )
    shape_id += 1

    title_paragraphs = [
        paragraph_xml("OmniMarket", size=3000, color=GOLD, font=TITLE_FONT, bold=True, align="ctr"),
        paragraph_xml("Apresentacao de Sprint", size=1900, color=WHITE, font=BODY_FONT, align="ctr"),
        paragraph_xml("Marketplace P2P para pequenos vendedores", size=1350, color=LIGHT, font=ACCENT_FONT, align="ctr"),
    ]
    shapes.append(
        text_shape_xml(
            shape_id,
            "Title",
            emu(2.4),
            emu(4.25),
            emu(8.5),
            emu(1.4),
            title_paragraphs,
            anchor="ctr",
        )
    )
    shape_id += 1

    footer = [
        paragraph_xml("TCC | Tecnico em Desenvolvimento de Sistemas | ETEC", size=1100, color=LIGHT, font=BODY_FONT, align="ctr"),
        paragraph_xml("[NOME DA EQUIPE]  |  [NOME DO ORIENTADOR]", size=1000, color=GOLD, font=BODY_FONT, align="ctr"),
    ]
    shapes.append(
        text_shape_xml(
            shape_id,
            "Footer",
            emu(2.25),
            emu(5.78),
            emu(8.85),
            emu(0.55),
            footer,
            anchor="ctr",
        )
    )

    return xml(
        f"""
        <p:sld xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main"
               xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"
               xmlns:p="http://schemas.openxmlformats.org/presentationml/2006/main">
          <p:cSld>
            <p:spTree>
              {group_shape_base()}
              {''.join(shapes)}
            </p:spTree>
          </p:cSld>
          <p:clrMapOvr>
            <a:masterClrMapping/>
          </p:clrMapOvr>
        </p:sld>
        """
    )


def build_content_slide(slide: SlideSpec, slide_number: int, total_slides: int) -> str:
    shapes: list[str] = []
    shape_id = 2

    shapes.append(shape_xml(shape_id, "Background", 0, 0, SLIDE_WIDTH, SLIDE_HEIGHT, fill=BG, line=None))
    shape_id += 1
    shapes.append(shape_xml(shape_id, "Top Line", emu(0.8), emu(0.48), emu(11.75), emu(0.04), fill=GOLD, line=None))
    shape_id += 1
    shapes.append(shape_xml(shape_id, "Note Box", emu(8.25), emu(2.1), emu(3.95), emu(2.8), fill=None, line=GOLD, preset="roundRect"))
    shape_id += 1

    shapes.append(
        picture_xml(
            shape_id,
            "Logo Small",
            emu(11.65),
            emu(0.7),
            emu(0.82),
            emu(0.82),
            "rId2",
        )
    )
    shape_id += 1

    shapes.append(
        text_shape_xml(
            shape_id,
            "Section",
            emu(0.95),
            emu(0.68),
            emu(2.9),
            emu(0.35),
            [paragraph_xml(slide.section, size=1300, color=GOLD, font=ACCENT_FONT, bold=True)],
        )
    )
    shape_id += 1

    shapes.append(
        text_shape_xml(
            shape_id,
            "Title",
            emu(0.95),
            emu(1.15),
            emu(6.7),
            emu(0.75),
            [paragraph_xml(slide.title, size=2500, color=WHITE, font=TITLE_FONT, bold=True)],
        )
    )
    shape_id += 1

    shapes.append(shape_xml(shape_id, "Title Underline", emu(0.98), emu(1.95), emu(2.15), emu(0.04), fill=GOLD, line=None))
    shape_id += 1

    bullet_paragraphs = [paragraph_xml(item, size=1700, color=LIGHT, font=BODY_FONT, bullet=True) for item in slide.bullets]
    shapes.append(
        text_shape_xml(
            shape_id,
            "Bullets",
            emu(1.0),
            emu(2.2),
            emu(6.75),
            emu(3.4),
            bullet_paragraphs,
        )
    )
    shape_id += 1

    note_paragraphs = [
        paragraph_xml(slide.note_title, size=1750, color=GOLD, font=TITLE_FONT, bold=True),
    ]
    note_paragraphs.extend(
        paragraph_xml(line, size=1400, color=LIGHT, font=BODY_FONT) for line in slide.note_lines
    )
    shapes.append(
        text_shape_xml(
            shape_id,
            "Notes",
            emu(8.5),
            emu(2.35),
            emu(3.45),
            emu(2.3),
            note_paragraphs,
        )
    )
    shape_id += 1

    shapes.append(
        text_shape_xml(
            shape_id,
            "Footer",
            emu(0.96),
            emu(6.15),
            emu(3.8),
            emu(0.3),
            [paragraph_xml("OmniMarket | Sprint", size=1000, color=GRAY, font=BODY_FONT)],
            anchor="ctr",
        )
    )
    shape_id += 1

    shapes.append(
        text_shape_xml(
            shape_id,
            "Page",
            emu(11.55),
            emu(6.1),
            emu(0.6),
            emu(0.3),
            [paragraph_xml(f"{slide_number}/{total_slides}", size=1000, color=GOLD, font=BODY_FONT, bold=True, align="ctr")],
            anchor="ctr",
        )
    )

    return xml(
        f"""
        <p:sld xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main"
               xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"
               xmlns:p="http://schemas.openxmlformats.org/presentationml/2006/main">
          <p:cSld>
            <p:spTree>
              {group_shape_base()}
              {''.join(shapes)}
            </p:spTree>
          </p:cSld>
          <p:clrMapOvr>
            <a:masterClrMapping/>
          </p:clrMapOvr>
        </p:sld>
        """
    )


def slide_rels_xml(include_logo: bool) -> str:
    logo_rel = ""
    if include_logo:
        logo_rel = """
          <Relationship
            Id="rId2"
            Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/image"
            Target="../media/logo.png"/>
        """.rstrip()

    return xml(
        f"""
        <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
          <Relationship
            Id="rId1"
            Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/slideLayout"
            Target="../slideLayouts/slideLayout1.xml"/>
          {logo_rel}
        </Relationships>
        """
    )


def content_types_xml(slide_count: int) -> str:
    slide_overrides = "\n".join(
        f'  <Override PartName="/ppt/slides/slide{index}.xml" ContentType="application/vnd.openxmlformats-officedocument.presentationml.slide+xml"/>'
        for index in range(1, slide_count + 1)
    )
    return xml(
        f"""
        <Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
          <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
          <Default Extension="xml" ContentType="application/xml"/>
          <Default Extension="png" ContentType="image/png"/>
          <Override PartName="/ppt/presentation.xml" ContentType="application/vnd.openxmlformats-officedocument.presentationml.presentation.main+xml"/>
          <Override PartName="/ppt/presProps.xml" ContentType="application/vnd.openxmlformats-officedocument.presentationml.presProps+xml"/>
          <Override PartName="/ppt/viewProps.xml" ContentType="application/vnd.openxmlformats-officedocument.presentationml.viewProps+xml"/>
          <Override PartName="/ppt/tableStyles.xml" ContentType="application/vnd.openxmlformats-officedocument.presentationml.tableStyles+xml"/>
          <Override PartName="/ppt/slideMasters/slideMaster1.xml" ContentType="application/vnd.openxmlformats-officedocument.presentationml.slideMaster+xml"/>
          <Override PartName="/ppt/slideLayouts/slideLayout1.xml" ContentType="application/vnd.openxmlformats-officedocument.presentationml.slideLayout+xml"/>
          <Override PartName="/ppt/theme/theme1.xml" ContentType="application/vnd.openxmlformats-officedocument.theme+xml"/>
          <Override PartName="/docProps/core.xml" ContentType="application/vnd.openxmlformats-package.core-properties+xml"/>
          <Override PartName="/docProps/app.xml" ContentType="application/vnd.openxmlformats-officedocument.extended-properties+xml"/>
          {slide_overrides}
        </Types>
        """
    )


def root_rels_xml() -> str:
    return xml(
        """
        <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
          <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="ppt/presentation.xml"/>
          <Relationship Id="rId2" Type="http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties" Target="docProps/core.xml"/>
          <Relationship Id="rId3" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties" Target="docProps/app.xml"/>
        </Relationships>
        """
    )


def app_xml(slide_count: int) -> str:
    return xml(
        f"""
        <Properties xmlns="http://schemas.openxmlformats.org/officeDocument/2006/extended-properties"
                    xmlns:vt="http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes">
          <Application>Microsoft Office PowerPoint</Application>
          <PresentationFormat>On-screen Show</PresentationFormat>
          <Slides>{slide_count}</Slides>
          <Notes>0</Notes>
          <HiddenSlides>0</HiddenSlides>
          <MMClips>0</MMClips>
          <ScaleCrop>false</ScaleCrop>
          <HeadingPairs>
            <vt:vector size="2" baseType="variant">
              <vt:variant><vt:lpstr>Theme</vt:lpstr></vt:variant>
              <vt:variant><vt:i4>1</vt:i4></vt:variant>
            </vt:vector>
          </HeadingPairs>
          <TitlesOfParts>
            <vt:vector size="1" baseType="lpstr">
              <vt:lpstr>Omni Theme</vt:lpstr>
            </vt:vector>
          </TitlesOfParts>
          <Company></Company>
          <LinksUpToDate>false</LinksUpToDate>
          <SharedDoc>false</SharedDoc>
          <HyperlinksChanged>false</HyperlinksChanged>
          <AppVersion>16.0000</AppVersion>
        </Properties>
        """
    )


def core_xml() -> str:
    timestamp = datetime.now(timezone.utc).replace(microsecond=0).isoformat().replace("+00:00", "Z")
    return xml(
        f"""
        <cp:coreProperties xmlns:cp="http://schemas.openxmlformats.org/package/2006/metadata/core-properties"
                           xmlns:dc="http://purl.org/dc/elements/1.1/"
                           xmlns:dcterms="http://purl.org/dc/terms/"
                           xmlns:dcmitype="http://purl.org/dc/dcmitype/"
                           xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
          <dc:title>OmniMarket - Apresentacao de Sprint</dc:title>
          <dc:subject>TCC OmniMarket</dc:subject>
          <dc:creator>OpenAI Codex</dc:creator>
          <cp:lastModifiedBy>OpenAI Codex</cp:lastModifiedBy>
          <dcterms:created xsi:type="dcterms:W3CDTF">{timestamp}</dcterms:created>
          <dcterms:modified xsi:type="dcterms:W3CDTF">{timestamp}</dcterms:modified>
        </cp:coreProperties>
        """
    )


def presentation_xml(slide_count: int) -> str:
    slide_ids = "\n".join(
        f'    <p:sldId id="{255 + index}" r:id="rId{1 + index}"/>'
        for index in range(1, slide_count + 1)
    )
    return xml(
        f"""
        <p:presentation xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main"
                        xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"
                        xmlns:p="http://schemas.openxmlformats.org/presentationml/2006/main"
                        saveSubsetFonts="1"
                        autoCompressPictures="0">
          <p:sldMasterIdLst>
            <p:sldMasterId id="2147483648" r:id="rId1"/>
          </p:sldMasterIdLst>
          <p:sldIdLst>
{slide_ids}
          </p:sldIdLst>
          <p:sldSz cx="{SLIDE_WIDTH}" cy="{SLIDE_HEIGHT}"/>
          <p:notesSz cx="6858000" cy="9144000"/>
          <p:defaultTextStyle>
            <a:defPPr>
              <a:defRPr lang="pt-BR"/>
            </a:defPPr>
          </p:defaultTextStyle>
        </p:presentation>
        """
    )


def presentation_rels_xml(slide_count: int) -> str:
    relationships = [
        '<Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/slideMaster" Target="slideMasters/slideMaster1.xml"/>'
    ]
    relationships.extend(
        f'<Relationship Id="rId{1 + index}" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/slide" Target="slides/slide{index}.xml"/>'
        for index in range(1, slide_count + 1)
    )
    relationships.append(
        f'<Relationship Id="rId{slide_count + 2}" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/presProps" Target="presProps.xml"/>'
    )
    relationships.append(
        f'<Relationship Id="rId{slide_count + 3}" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/viewProps" Target="viewProps.xml"/>'
    )
    relationships.append(
        f'<Relationship Id="rId{slide_count + 4}" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/tableStyles" Target="tableStyles.xml"/>'
    )
    return xml(
        f"""
        <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
          {' '.join(relationships)}
        </Relationships>
        """
    )


def pres_props_xml() -> str:
    return xml(
        """
        <p:presentationPr xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main"
                          xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"
                          xmlns:p="http://schemas.openxmlformats.org/presentationml/2006/main">
          <p:showPr useTimings="0" browse="0" loop="0" showNarration="1"/>
        </p:presentationPr>
        """
    )


def view_props_xml() -> str:
    return xml(
        """
        <p:viewPr xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main"
                  xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"
                  xmlns:p="http://schemas.openxmlformats.org/presentationml/2006/main"
                  lastView="sldView">
          <p:normalViewPr>
            <p:restoredLeft sz="15620" autoAdjust="0"/>
            <p:restoredTop sz="94660" autoAdjust="0"/>
          </p:normalViewPr>
          <p:slideViewPr>
            <p:cSldViewPr snapToGrid="0" snapToObjects="1" showGuides="0" showGrid="0"/>
          </p:slideViewPr>
          <p:notesTextViewPr>
            <p:cViewPr varScale="100"/>
          </p:notesTextViewPr>
          <p:gridSpacing cx="914400" cy="914400"/>
        </p:viewPr>
        """
    )


def table_styles_xml() -> str:
    return xml(
        """
        <a:tblStyleLst xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main"
                       def="{5C22544A-7EE6-4342-B048-85BDC9FD1C3A}"/>
        """
    )


def slide_master_xml() -> str:
    return xml(
        """
        <p:sldMaster xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main"
                     xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"
                     xmlns:p="http://schemas.openxmlformats.org/presentationml/2006/main">
          <p:cSld name="OmniMarket Master">
            <p:spTree>
              <p:nvGrpSpPr>
                <p:cNvPr id="1" name=""/>
                <p:cNvGrpSpPr/>
                <p:nvPr/>
              </p:nvGrpSpPr>
              <p:grpSpPr>
                <a:xfrm>
                  <a:off x="0" y="0"/>
                  <a:ext cx="0" cy="0"/>
                  <a:chOff x="0" y="0"/>
                  <a:chExt cx="0" cy="0"/>
                </a:xfrm>
              </p:grpSpPr>
            </p:spTree>
          </p:cSld>
          <p:clrMap bg1="lt1" tx1="dk1" bg2="lt2" tx2="dk2" accent1="accent1" accent2="accent2" accent3="accent3" accent4="accent4" accent5="accent5" accent6="accent6" hlink="hlink" folHlink="folHlink"/>
          <p:sldLayoutIdLst>
            <p:sldLayoutId id="1" r:id="rId1"/>
          </p:sldLayoutIdLst>
        </p:sldMaster>
        """
    )


def slide_master_rels_xml() -> str:
    return xml(
        """
        <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
          <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/slideLayout" Target="../slideLayouts/slideLayout1.xml"/>
          <Relationship Id="rId2" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme" Target="../theme/theme1.xml"/>
        </Relationships>
        """
    )


def slide_layout_xml() -> str:
    return xml(
        """
        <p:sldLayout xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main"
                     xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"
                     xmlns:p="http://schemas.openxmlformats.org/presentationml/2006/main"
                     type="blank"
                     preserve="1">
          <p:cSld name="Blank">
            <p:spTree>
              <p:nvGrpSpPr>
                <p:cNvPr id="1" name=""/>
                <p:cNvGrpSpPr/>
                <p:nvPr/>
              </p:nvGrpSpPr>
              <p:grpSpPr>
                <a:xfrm>
                  <a:off x="0" y="0"/>
                  <a:ext cx="0" cy="0"/>
                  <a:chOff x="0" y="0"/>
                  <a:chExt cx="0" cy="0"/>
                </a:xfrm>
              </p:grpSpPr>
            </p:spTree>
          </p:cSld>
          <p:clrMapOvr>
            <a:masterClrMapping/>
          </p:clrMapOvr>
        </p:sldLayout>
        """
    )


def slide_layout_rels_xml() -> str:
    return xml(
        """
        <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
          <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/slideMaster" Target="../slideMasters/slideMaster1.xml"/>
        </Relationships>
        """
    )


def theme_xml() -> str:
    return xml(
        f"""
        <a:theme xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main" name="Omni Theme">
          <a:themeElements>
            <a:clrScheme name="Omni">
              <a:dk1><a:srgbClr val="{BG}"/></a:dk1>
              <a:lt1><a:srgbClr val="{WHITE}"/></a:lt1>
              <a:dk2><a:srgbClr val="{GRAY}"/></a:dk2>
              <a:lt2><a:srgbClr val="{LIGHT}"/></a:lt2>
              <a:accent1><a:srgbClr val="{GOLD}"/></a:accent1>
              <a:accent2><a:srgbClr val="{LIGHT}"/></a:accent2>
              <a:accent3><a:srgbClr val="{GRAY}"/></a:accent3>
              <a:accent4><a:srgbClr val="{WHITE}"/></a:accent4>
              <a:accent5><a:srgbClr val="{GOLD}"/></a:accent5>
              <a:accent6><a:srgbClr val="{LIGHT}"/></a:accent6>
              <a:hlink><a:srgbClr val="{GOLD}"/></a:hlink>
              <a:folHlink><a:srgbClr val="{LIGHT}"/></a:folHlink>
            </a:clrScheme>
            <a:fontScheme name="Omni">
              <a:majorFont>
                <a:latin typeface="{TITLE_FONT}"/>
                <a:ea typeface="{TITLE_FONT}"/>
                <a:cs typeface="{TITLE_FONT}"/>
              </a:majorFont>
              <a:minorFont>
                <a:latin typeface="{BODY_FONT}"/>
                <a:ea typeface="{BODY_FONT}"/>
                <a:cs typeface="{BODY_FONT}"/>
              </a:minorFont>
            </a:fontScheme>
            <a:fmtScheme name="Omni">
              <a:fillStyleLst>
                <a:solidFill><a:schemeClr val="phClr"/></a:solidFill>
                <a:solidFill><a:srgbClr val="{BG}"/></a:solidFill>
                <a:solidFill><a:srgbClr val="{LIGHT}"/></a:solidFill>
              </a:fillStyleLst>
              <a:lnStyleLst>
                <a:ln w="9525"><a:solidFill><a:srgbClr val="{GOLD}"/></a:solidFill></a:ln>
                <a:ln w="25400"><a:solidFill><a:srgbClr val="{GOLD}"/></a:solidFill></a:ln>
                <a:ln w="38100"><a:solidFill><a:srgbClr val="{GOLD}"/></a:solidFill></a:ln>
              </a:lnStyleLst>
              <a:effectStyleLst>
                <a:effectStyle><a:effectLst/></a:effectStyle>
                <a:effectStyle><a:effectLst/></a:effectStyle>
                <a:effectStyle><a:effectLst/></a:effectStyle>
              </a:effectStyleLst>
              <a:bgFillStyleLst>
                <a:solidFill><a:srgbClr val="{BG}"/></a:solidFill>
                <a:solidFill><a:srgbClr val="{GRAY}"/></a:solidFill>
                <a:solidFill><a:srgbClr val="{LIGHT}"/></a:solidFill>
              </a:bgFillStyleLst>
            </a:fmtScheme>
          </a:themeElements>
          <a:objectDefaults/>
          <a:extraClrSchemeLst/>
        </a:theme>
        """
    )


def build_presentation(output_path: Path) -> None:
    logo_path = output_path.parent / "prototype_extract" / "image1.png"
    if not logo_path.exists():
        raise FileNotFoundError(f"Logo nao encontrado em: {logo_path}")

    slides = [build_title_slide()]
    total_slides = len(SLIDES) + 1
    for idx, slide in enumerate(SLIDES, start=2):
        slides.append(build_content_slide(slide, idx, total_slides))

    with ZipFile(output_path, "w", compression=ZIP_DEFLATED) as archive:
        archive.writestr("[Content_Types].xml", content_types_xml(len(slides)))
        archive.writestr("_rels/.rels", root_rels_xml())
        archive.writestr("docProps/app.xml", app_xml(len(slides)))
        archive.writestr("docProps/core.xml", core_xml())
        archive.writestr("ppt/presentation.xml", presentation_xml(len(slides)))
        archive.writestr("ppt/_rels/presentation.xml.rels", presentation_rels_xml(len(slides)))
        archive.writestr("ppt/presProps.xml", pres_props_xml())
        archive.writestr("ppt/viewProps.xml", view_props_xml())
        archive.writestr("ppt/tableStyles.xml", table_styles_xml())
        archive.writestr("ppt/theme/theme1.xml", theme_xml())
        archive.writestr("ppt/slideMasters/slideMaster1.xml", slide_master_xml())
        archive.writestr("ppt/slideMasters/_rels/slideMaster1.xml.rels", slide_master_rels_xml())
        archive.writestr("ppt/slideLayouts/slideLayout1.xml", slide_layout_xml())
        archive.writestr("ppt/slideLayouts/_rels/slideLayout1.xml.rels", slide_layout_rels_xml())
        archive.writestr("ppt/media/logo.png", logo_path.read_bytes())

        for index, slide_xml in enumerate(slides, start=1):
            archive.writestr(f"ppt/slides/slide{index}.xml", slide_xml)
            archive.writestr(f"ppt/slides/_rels/slide{index}.xml.rels", slide_rels_xml(include_logo=True))


def main() -> None:
    output_dir = Path(__file__).resolve().parent
    output_path = output_dir / "OmniMarket_Apresentacao_Sprint.pptx"
    build_presentation(output_path)
    print(output_path)


if __name__ == "__main__":
    main()
