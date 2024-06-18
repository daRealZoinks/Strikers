using UnityEngine;

public class BallColorManager : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private Goal blueGoal;
    [SerializeField] private Goal orangeGoal;

    [SerializeField] private Color blueColor = new(0.0f, 0.36f, 1.0f);
    [SerializeField] private Color middleColor = Color.white;
    [SerializeField] private Color orangeColor = new(1.0f, 0.54f, 0.0f);
    [SerializeField] private float emission = 3f;

    private void Start()
    {
        material.color = middleColor;
    }

    private void Update()
    {
        var distanceToBlueGoal = Vector3.Distance(blueGoal.transform.position, transform.position);
        var distanceToOrangeGoal = Vector3.Distance(orangeGoal.transform.position, transform.position);

        var blueLerp = Color.Lerp(middleColor, blueColor, transform.position.z / blueGoal.transform.position.z);
        var orangeLerp = Color.Lerp(middleColor, orangeColor, transform.position.z / orangeGoal.transform.position.z);

        var finalColor = distanceToBlueGoal < distanceToOrangeGoal ? blueLerp : orangeLerp;

        material.color = finalColor;
        material.SetVector("_EmissionColor", finalColor * emission);
    }
}