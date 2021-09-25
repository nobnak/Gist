namespace nobnak.Gist {

	public interface IInvalidatable {
		void Invalidate();
	}
	public interface IValidator : IInvalidatable {
		bool IsValid { get; }

		bool Validate(bool force = false);
	}
}