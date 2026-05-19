// Cria blocos visuais simples para indicar carregamento da pagina.
interface ProfileSkeletonProps {
  lines?: number;
  cardCount?: number;
}

export function ProfileSkeleton({
  lines = 3,
  cardCount = 3,
}: ProfileSkeletonProps) {
  return (
    <div className="animate-pulse space-y-5">
      <div className="space-y-3">
        {Array.from({ length: lines }).map((_, index) => (
          <div
            key={`line-${index}`}
            className="h-4 rounded-full bg-white/8"
            style={{ width: `${100 - index * 12}%` }}
          />
        ))}
      </div>

      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
        {Array.from({ length: cardCount }).map((_, index) => (
          <div
            key={`card-${index}`}
            className="overflow-hidden rounded-2xl border border-white/10 bg-black/45"
          >
            <div className="h-40 bg-white/8" />
            <div className="space-y-3 p-4">
              <div className="h-4 w-3/4 rounded-full bg-white/8" />
              <div className="h-4 w-1/2 rounded-full bg-white/8" />
              <div className="h-5 w-1/3 rounded-full bg-yellow-400/20" />
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
