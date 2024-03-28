using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.Flex;
using System.IO; // for writing to file

public class ControlSimulations : MonoBehaviour
{
    public float sim_time = 0.0f;
    private float t_max = 14.0f;
    public GameObject crackObject;
    SelectFluidParameters fluidParametersSelector = new SelectFluidParameters();
    CaptureParticlePositions positionCapturer = new CaptureParticlePositions();
    private string pythonFilePath = "python_output";
    private NVIDIA.Flex.FlexSourceActor sourceActorScript;
    public bool waitingForFile = true;
    private bool nozzleMoving = false;
    public GameObject nozzleTarget; // sphere representing the target point
    
    public float nozzleSpeed = 0.384f;

    // Start is called before the first frame update
    void Start()
    {
        sourceActorScript = gameObject.GetComponent<NVIDIA.Flex.FlexSourceActor>(); // find the fluid source script attached to the same object as this script
        sourceActorScript.isActive = false; // turn off fluid source
    }

    // Update is called once per frame
    void Update()
    {
        if (waitingForFile) waitForFile(); // wait for a new python_output file to be created from the Python script
        else simulationLoop();
    }

    void waitForFile()
    {
        bool gotFile = false;
        gotFile = fluidParametersSelector.FluidParametersFromFile(sourceActorScript, pythonFilePath, true); // this outputs true if it found the python_output file and set the fluid parameters
        if (gotFile) StartSimulation();
    }

    void StartSimulation()
    {
            nozzleMoving = true; // start nozzle moving
            sourceActorScript.isActive = true; // turn on fluid source
            waitingForFile = false;
            sim_time = 0.0f;
    }

    void simulationLoop()
    {
        float dT = Application.isPlaying ? Time.deltaTime : 0; // get the simulation timestep
        sim_time += dT;

        if (sim_time % 1.0f < dT) // only check every 1 second to save computation
        {
            if (!CheckParticlesMoving()) EndSimulation(); // check if anything is still moving, if not then end the simulation
        }

        if (sim_time > t_max) EndSimulation(); // simulation finished

        if (nozzleMoving)
        {
            if (nozzleController.MoveNozzle(sourceActorScript, gameObject, nozzleTarget, nozzleSpeed, dT)) { // this outputs true if the nozzle has reached the destination. Otherwise it just moved the nozzle
                nozzleMoving = false;
                sourceActorScript.isActive = false; // turn off fluid source
            }
        }
    }

    public void EndSimulation()
    {
        string trials_output_filepath = "Assets/Resources/unity_output"; // this is the same name as references in the Bayesian Optimisation script
        positionCapturer.SaveParticlePositions(sourceActorScript, trials_output_filepath); // save particle positions to unity_output file 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // restart the simulation - it will then wait for a new python_output file to be created from the Python script
    }

    bool CheckParticlesMoving()
    {
        Vector3[] m_velocityArray = positionCapturer.GetParticleVelocities(sourceActorScript);
        float maxSpeed = 0.0f;
        foreach (Vector3 velocity in m_velocityArray)
        {
            if (velocity.magnitude > maxSpeed) maxSpeed = velocity.magnitude;
        }
        if (maxSpeed < 0.5f) return false;
        return true;
    }
}
