using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreAnimationHandler : MonoBehaviour
{
	//References
	[Header("UI references")]
	public GameObject scoreTextPrefab;
	public Transform target, startPoint, _dynamic;

	[Space]
	[Header("Available amount : (object's amount to pool)")]
	[SerializeField] int maxAmount;
	Queue<GameObject> currentQueue = new Queue<GameObject>();

	[Space]
	[Header("Animation settings")]
	[SerializeField] [Range(0.5f, 0.9f)] float minAnimDuration;
	[SerializeField] [Range(0.9f, 2f)] float maxAnimDuration;

	[SerializeField] Ease easeType;
	[SerializeField] float spread;

	void Awake()
	{
		//prepare pool
		PrepareObjectPool();
	}

	void PrepareObjectPool()
	{
		GameObject obj;
		for (int i = 0; i < maxAmount; i++)
		{
			obj = Instantiate(scoreTextPrefab);
			obj.transform.SetParent(_dynamic);
			obj.SetActive(false);
			currentQueue.Enqueue(obj);
		}
	}

	void Animate(Vector3 collectedCoinPosition, int amount, int score)
	{
		for (int i = 0; i < amount; i++)
		{
			//check if there's coins in the pool
			if (currentQueue.Count > 0)
			{
				//extract a coin from the pool
				GameObject obj = currentQueue.Dequeue();
				obj.GetComponent<TextMeshProUGUI>().text = "+" + score.ToString();
				obj.SetActive(true);

				//move coin to the collected coin pos
				obj.transform.position = collectedCoinPosition + new Vector3(Random.Range(-spread, spread), 0f, 0f);

				//animate coin to target position
				float duration = Random.Range(minAnimDuration, maxAnimDuration);
				obj.transform.DOMove(target.position, duration)
				.SetEase(easeType)
				.OnComplete(() => {
					//executes whenever coin reach target position
					obj.SetActive(false);
					currentQueue.Enqueue(obj);
					MainGameSceneController.Instance.UpdateScore(score);
				});
			}
		}
	}

	public void AddScoreAnimation(Vector3 spawnPosition, int amount, int score)
	{
		Animate(spawnPosition, amount, score);
	}
}