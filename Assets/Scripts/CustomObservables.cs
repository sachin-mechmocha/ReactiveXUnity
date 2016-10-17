using UniRx;

public static class CustomObservables {
	
	public static IObservable<bool> Latch (IObservable<Unit> tick, IObservable<Unit> latchTrue, bool initialValue) {
		return Observable.Create<bool>(observer => {
			var value = initialValue;
			var latchSub = latchTrue.Subscribe(_ => value = true);
			var tickSub = tick.Subscribe ( _=> {
								observer.OnNext (value);
								value = false;
							},
							observer.OnError,
							observer.OnCompleted);
			return Disposable.Create (()=> {
				latchSub.Dispose ();
				tickSub.Dispose ();
			});
		});
	}
}
