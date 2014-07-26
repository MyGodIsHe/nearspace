using System;


public class State {

	static bool isPause = false;

	public static bool IsPause {
		get {
			return isPause;
		}
	}

	public static void Stop() {
		isPause = true;
	}
	
	public static void Play() {
		isPause = false;
	}
}
