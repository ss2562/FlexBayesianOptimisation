using UnityEngine;

[System.Serializable]
public class FluidParameters
{
    // stores the fluid parameters as float
    // e.g. FluidParameters params = CreateFromJSON(string jsonString)
    // to access the values in params, we simply use params.[parameter name]
    // Debug.Log(params.cohesion) -> "0.01"
    // Debug.Log(params.viscosity) -> "0.03"
    public float cohesion;
    public float surfaceTension;
    public float viscosity;
    public float adhesion;

    public static FluidParameters CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<FluidParameters>(jsonString);
    }

    // Given JSON input:
    // {"viscosity":10.5}
    // this example will return a FluidParameters object with
    // viscosity == 10.5
}