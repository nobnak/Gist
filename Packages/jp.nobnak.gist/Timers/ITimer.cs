using System;

namespace nobnak.Gist {
	public interface ITimer {
		bool Active { get; }
		bool Completed { get; }
		float ElapsedTime { get; }
		float Interval { get; set; }

		event Action<Timer> Elapsed;

		void Start();
		void Start(float interval);
		void Stop();
		bool Update();
	}
}