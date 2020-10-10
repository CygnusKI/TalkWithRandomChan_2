using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeTest : MonoBehaviour
{
	[SerializeField]
	Fade fade = null;

	public void OnClick()
	{
		fade.FadeIn (1, () =>
		{
			fade.FadeOut(1);
		});
	}
}
