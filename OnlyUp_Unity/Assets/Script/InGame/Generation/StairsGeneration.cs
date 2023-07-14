using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsGeneration : MonoBehaviour
{
    [SerializeField] private GameObject stairs;//階段オブジェクト
    private float radius = 8f;           // 螺旋の半径
    private float height = 10f;           // 螺旋の高さ
    public int numberOfGeneration;  //生成する数
    private float spawnOffset = 1f;
    private float rotationSpeed = 0f;

    void Start()
    {
        //螺旋階段の生成
        for(var i = 0; i < numberOfGeneration; i++)
        {
            float angle = i * (transform.rotation.y + 2f * Mathf.PI / numberOfGeneration);
            float x = Mathf.Cos(angle) * radius - 1f;
            float y = i * height / numberOfGeneration + 0.5f;
            float z = Mathf.Sin(angle) * radius;

            Vector3 position = new Vector3(x, y, z);
            Quaternion rotation = Quaternion.Euler(0f, angle * Mathf.Rad2Deg + 10f, 0f);

            GameObject spawnedObject = Instantiate(stairs, position, rotation);
            spawnedObject.transform.parent = transform;
        }
        
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    
}
