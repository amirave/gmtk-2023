using System.Threading;
using Cysharp.Threading.Tasks;

namespace GameManager
{
    public abstract class GameStateBase<TContext, TConfig> : IGameState
        where TContext : class where TConfig : class
    {
        protected TContext context => _context;
        protected TConfig config => _config;
        public bool isSuspended => _suspended;
        public bool isRunning => _isRunning;
        
        private TContext _context;
        private TConfig _config;
        private bool _suspended;
        private bool _isRunning;

        public UniTask Run(TContext ctx, TConfig conf, CancellationToken cancellationToken)
        {
            _isRunning = true;
            _context = ctx;
            _config = conf;
            
            return OnRun(cancellationToken);
        }
        
        public void Suspend()
        {
            _suspended = true;
            OnSuspend();
        }

        public void Resume()
        {
            _suspended = false;
            OnResume();
        }
        
        protected abstract UniTask OnRun(CancellationToken cancellationToken);
        protected abstract void OnSuspend();
        protected abstract void OnResume();
    }
}