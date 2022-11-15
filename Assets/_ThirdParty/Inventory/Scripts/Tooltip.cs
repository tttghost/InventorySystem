using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
	private ItemType itemType;
	private string data;
	private GameObject tooltip;

	void Start()
	{
		tooltip = GameObject.Find("go_Tooltip");
		tooltip.SetActive(false);
	}

	void Update()
	{
		if (tooltip.activeSelf)
		{
			tooltip.transform.position = Input.mousePosition;
		}
	}

	public void Activate(ItemType itemType)
	{
		this.itemType = itemType;
		ConstructDataString();
		tooltip.SetActive(true);
	}

	public void Deactivate()
	{
		tooltip.SetActive(false);
	}

	public void ConstructDataString()
	{
		data = "<color=#FFEC58FF><b>" + itemType.title+ "</b></color>\n\n" + itemType.description
			+ "\nPower: " + itemType.slug;
		tooltip.transform.GetChild(0).GetComponent<Text>().text = data;
	}

}
