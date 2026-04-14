const ErrorBoundary = ({ title = 'Error', message = 'Something went wrong.', onRetry, onGoHome }) => {
  return (
    <div className="rounded-lg border border-red-200 bg-red-50 p-6">
      <h2 className="text-lg font-semibold text-red-800">{title}</h2>
      <p className="mt-2 text-sm text-red-700">{message}</p>
      <div className="mt-4 flex gap-2">
        {onRetry ? (
          <button type="button" onClick={onRetry} className="rounded bg-red-600 px-3 py-2 text-sm text-white">
            Retry
          </button>
        ) : null}
        {onGoHome ? (
          <button type="button" onClick={onGoHome} className="rounded border border-red-300 px-3 py-2 text-sm text-red-700">
            Go Home
          </button>
        ) : null}
      </div>
    </div>
  );
};

export default ErrorBoundary;
