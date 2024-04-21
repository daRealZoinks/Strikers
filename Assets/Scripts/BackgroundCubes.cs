using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BackgroundCubes : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private int cubeCount = 1000;
    [SerializeField] private float cubeDistance = 200f;
    [SerializeField] private float cubeYPositionMin = -70f;
    [SerializeField] private float cubeYPositionMax = 100f;
    [SerializeField] private float cubeSizeMin = 3f;
    [SerializeField] private float cubeSizeMax = 10f;
    [SerializeField] private float cubeSpeedMin = -3f;
    [SerializeField] private float cubeSpeedMax = 3f;

    [Serializable]
    private struct Cube
    {
        public GameObject GameObject;
        public float Speed;
    }

    private Cube[] cubes;

    private void Start()
    {
        cubes = new Cube[cubeCount];

        for (var i = 0; i < cubeCount; i++)
        {
            var cube = cubes[i];

            cube.GameObject = Instantiate(cubePrefab, transform);

            var randomScaleX = Random.Range(cubeSizeMin, cubeSizeMax);
            var randomScaleY = Random.Range(cubeSizeMin, cubeSizeMax);
            var randomScaleZ = Random.Range(cubeSizeMin, cubeSizeMax);
            cube.GameObject.transform.localScale = new Vector3(randomScaleX, randomScaleY, randomScaleZ);

            var randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            randomDirection *= cubeDistance;
            var randomY = Random.Range(cubeYPositionMin, cubeYPositionMax);
            cube.GameObject.transform.position = new Vector3(randomDirection.x, randomY, randomDirection.z);

            cube.GameObject.transform.rotation =
                Quaternion.LookRotation(transform.position - cube.GameObject.transform.position);
            cube.GameObject.transform.rotation =
                Quaternion.Euler(0, cube.GameObject.transform.rotation.eulerAngles.y, 0);

            cube.Speed = Random.Range(cubeSpeedMin, cubeSpeedMax);

            cubes[i] = cube;
        }
    }

    private void Update()
    {
        for (var i = 0; i < cubeCount; i++)
        {
            var cube = cubes[i];

            cube.GameObject.transform
                .RotateAround(
                    transform.position,
                    Vector3.up,
                    cube.Speed *
                    Time.deltaTime);
        }
    }
}