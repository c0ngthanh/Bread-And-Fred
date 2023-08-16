using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Helper;

public class ShootTrueChallenge : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] List<Element> element;
    [SerializeField] List<int> numberofBullet;


    [SerializeField] TMP_Text text;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject door;
    private int numberNow;
    private int indexNow = 0;
    private Element elementNow;
    // Start is called before the first frame update
    void Start()
    {
        text.color = ElementColor(element[0]);
        text.text = numberofBullet[0].ToString();
        numberNow = numberofBullet[indexNow];
        elementNow = element[indexNow];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Bullet>() != null)
        {
            Debug.Log("collider");
            if (other.gameObject.GetComponent<Bullet>().GetElement() == elementNow)
            {
                numberNow--;
                text.text = numberNow.ToString();
                if (numberNow == 0)
                {
                    indexNow++;
                    if (indexNow < element.Count)
                    {
                        numberNow = numberofBullet[indexNow];
                        elementNow = element[indexNow];
                        text.color = ElementColor(elementNow);
                        text.text = numberNow.ToString();
                    }
                    else
                    {
                        DoneChallenge();
                    }
                }
            }
            else
            {
                indexNow = 0;
                elementNow = element[indexNow];
                numberNow = numberofBullet[indexNow];
                text.color = ElementColor(elementNow);
                text.text = numberNow.ToString();
            }
            Destroy(other.gameObject);
        }
    }

    private void DoneChallenge()
    {
        text.gameObject.SetActive(false);
        spriteRenderer.color = Color.yellow;
        door.SetActive(false);
    }

    private Color ElementColor(Element element)
    {
        switch (element)
        {
            case Element.Fire: return Color.red;
            case Element.Water: return Color.blue;
            case Element.Elec: return new Color(0.509804f, 0, 1);
            case Element.Dendro: return Color.green;
        }
        return Color.white;
    }
}
