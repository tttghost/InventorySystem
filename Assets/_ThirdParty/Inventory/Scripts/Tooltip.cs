using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
	private GameObject tooltip;

	private void Start()
	{
		tooltip = GameObject.Find("go_Tooltip");
		Deactivate();
	}

	private void Update()
	{
		if (tooltip.activeSelf)
		{
			tooltip.transform.position = Input.mousePosition;
		}
	}

	public void Activate(ItemType itemType)
	{
		ConstructDataString(itemType);
		tooltip.SetActive(true);
	}

	public void Deactivate()
	{
		tooltip.SetActive(false);
	}

	private void ConstructDataString(ItemType itemType)
	{
		string data = "<color=#FFEC58FF><b>" 
			+ itemType.title
			+ "</b></color>\n\n" 
			+ itemType.description
			+ "\nPower: " + itemType.slug;
		tooltip.transform.GetChild(0).GetComponent<Text>().text = data;
	}

}
