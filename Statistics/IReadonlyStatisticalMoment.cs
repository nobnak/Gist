namespace nobnak.Gist.Statistics {

	public interface IReadonlyStatisticalMoment {
		float Average { get; }
		int Count { get; }
		float SD { get; }
		float UnbiasedVariance { get; }
	}
}