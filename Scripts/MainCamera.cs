using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float deathZoom = 5;
    [SerializeField] Vector3 distance;
    [SerializeField] GameObject canvas;

    void Update()
    {
        if (player == null)
        {
            Camera.main.orthographicSize = deathZoom;
            deathZoom += 0.05f;
            Vector3 position = new Vector2(transform.position.x, transform.position.y);
            distance = new Vector3(0f, 0f) - position;
            distance = distance / 100;
            canvas.SetActive(false);
        }
        if(distance != null)
        {
            transform.position += distance;
        }
    }
}
