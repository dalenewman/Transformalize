using System;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	public class ReindexObserver<T> : IObserver<IReindexResponse<T>> where T : class
	{
		private readonly Action<IReindexResponse<T>> _onNext;
		private readonly Action<System.Exception> _onError;
		private readonly Action _completed;

		public ReindexObserver(Action<IReindexResponse<T>> onNext = null, Action<System.Exception> onError = null, Action completed = null)
		{
			this._completed = completed;
			this._onError = onError;
			this._onNext = onNext;
		}


		public void OnCompleted()
		{
			if (this._completed != null)
				this._completed();
		}

		public void OnError(System.Exception error)
		{
			if (this._onError != null)
				this._onError(error);
		}

		public void OnNext(IReindexResponse<T> value)
		{
			if(this._onNext != null)
				this._onNext(value);
		}
	}
}