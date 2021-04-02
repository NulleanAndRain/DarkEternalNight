using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{

	private static GlobalSettings _instance;
	public bool _steppingUpEndabled;
	public bool _damageParticleStackingEnabled;

	public static bool steppingUpEndabled { get => _instance._steppingUpEndabled; set => _instance._steppingUpEndabled = value; }
	public static bool particlesStackingEnabled { get => _instance._damageParticleStackingEnabled; set => _instance._damageParticleStackingEnabled = value; }

	void Start() {
		if (_instance != null) Destroy(gameObject);

		_instance = this;
    }

	
}
