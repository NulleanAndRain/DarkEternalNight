using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScoreCollector : MonoBehaviour {
	public Vector2 ParticleEmitPos;
	public Vector2 ParticleEmitPoint { get => (Vector2)transform.position + ParticleEmitPos; }
	public float Score { get; private set; }

	public GameObject scoreParticle;

	public event Action<float> onScoreUpdate = delegate { };

	private float lastParticleScore;
	private float lastParticleTime;
	private TextParticle lastParticle;
	private GradientText lastPartColor;
	public float scoreUpdateTime;

	public static Color PosSidesColor = new Color32(0x85, 0x85, 0xDA, 0xFF);
	public static Color PosCenterColor = new Color32(0xCE, 0xD2, 0xEC, 0xFF);

	public static Color NegSidesColor = new Color32(0xFF, 0x33, 0x00, 0xFF);
	public static Color NegCenterColor = new Color32(0xFF, 0x7F, 0x00, 0xFF);
	private void changeScore(float score) {
		Score += score;

		if (Time.time - lastParticleTime > scoreUpdateTime || lastParticle == null || !GlobalSettings.particlesStackingEnabled) {
			lastParticleScore = score;
			lastParticle = TextParticle.Instantiate(scoreParticle, ParticleEmitPoint, HUD.floatToString(lastParticleScore));
			lastPartColor = lastParticle.GetComponent<GradientText>();
		} else {
			lastParticleScore += score;
			lastParticle.updateText(HUD.floatToString(lastParticleScore));
		}
		if (lastParticleScore != 0) {
			if (lastParticleScore > 0) {
				lastPartColor.ColorLeft = PosSidesColor;
				lastPartColor.ColorMid = PosCenterColor;
				lastPartColor.ColorRight = lastPartColor.ColorLeft;
		} else {
				lastPartColor.ColorLeft = NegSidesColor;
				lastPartColor.ColorMid = NegCenterColor;
				lastPartColor.ColorRight = lastPartColor.ColorLeft;
			}
		}

		lastParticleTime = Time.time;

		onScoreUpdate(Score);
	}

	public void CollectScore(float score) {
		changeScore(score);
	}

	public void LoseScore(float score) {
		changeScore(-score);
	}

	/// <summary>
	/// Substracts nearest multiple of 5 amount of score to given percent and returns substracted amount
	/// </summary>
	/// <param name="percents"></param>
	/// <returns></returns>
	public float LoseScorePecents(float percents) {
		float res = Mathf.Ceil(Score * percents / 5) * 5;
		LoseScore(res);
		return res;
	}

	/// <summary>
	/// Same as <method>LoseScorePecents</method>, but random in range
	/// </summary>
	/// <param name="min"></param>
	/// <param name="max"></param>
	/// <returns></returns>
	public float LoseScorePecentsRange(float min, float max) {
		return LoseScorePecents(Random.Range(min, max));
	}
}
