namespace nobnak.Gist {

	public interface IValidator {
		bool IsValid { get; }
		bool Validate(bool force = false);
	}
	public interface IInvalidatable : IValidator {
		void Invalidate();
	}
}