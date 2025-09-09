using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
public enum vehicleType
{
    RccCars,
    Bike,
    Train,
    AeroPlane,
    Other,
    Thirdperson,
}
public class vehiclescript : MonoBehaviour
{
    public vehicleType currentVehicle = vehicleType.RccCars;
    private GameManger gamePlay;
    private GameObject CollidedCar;
    public BasicBehaviour TPSChar;

    public Transform spawnPoint;
    void Start()
    {
        gamePlay = FindObjectOfType<GameManger>();
        gamePlay.Button1.onClick.AddListener(SwitchControls);
        gamePlay.ExitBtn.onClick.AddListener(CarToTps);
        TPSChar = FindObjectOfType<BasicBehaviour>();

        spawnPoint = transform.Find("spawnPoint");
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.LogError("Collision with: " + collision.gameObject.name);
            CollidedCar = collision.gameObject;
            gamePlay.Button1.gameObject.SetActive(true);
        }
    }
    public void CarToTps()
    {
        foreach (GameObject carControls in gamePlay.RCCControls)
        {
            carControls.SetActive(false);
        }
        gamePlay.trafficObject.transform.SetParent(gamePlay.player.transform);
        gamePlay.trafficObject.transform.localPosition = Vector3.zero;
        gamePlay.trafficObject.transform.localRotation = Quaternion.identity;


        gamePlay.ThirdPersonCntrol.SetActive(true);

        TPSChar = FindObjectOfType<BasicBehaviour>();

        TPSChar.transform.position = spawnPoint.position;
        TPSChar.transform.rotation = spawnPoint.rotation;

        //if (currentVehicle == vehicleType.Bike)
        //{
        //    this.GetComponent<BikeControl>().enabled = false;
        //    gamePlay.BikeCamera.SetActive(false);
        //    gamePlay.BikeControl.SetActive(false);
        //}
        //else if (currentVehicle == vehicleType.RccCars)
        //{
        //    this.GetComponent<RCC_CarControllerV3>().enabled = false;
        //    this.GetComponent<Rigidbody>().isKinematic = true;
        //    this.GetComponent<RCC_CarControllerV3>().KillEngine();
        //    this.transform.GetChild(1).gameObject.SetActive(true);
        //}
        //else
        //{
        //    Debug.Log("no condition");
        //}

        TPSChar.gameObject.AddComponent<vehiclescript>();
        TPSChar.gameObject.GetComponent<vehiclescript>().currentVehicle = vehicleType.Thirdperson;

        Destroy(this.GetComponent<vehiclescript>());

        gamePlay.ExitBtn.gameObject.SetActive(false);
    }
    public void SwitchControls() => StartCoroutine(SwitchControlsC());
    IEnumerator SwitchControlsC()
    {
        switch (currentVehicle)
        {
            case vehicleType.RccCars:
                this.GetComponent<RCC_CarControllerV3>().enabled = false;
                this.transform.GetChild(1).gameObject.SetActive(true);

                yield return new WaitForSeconds(1f);

                CollidedCar.GetComponent<RCC_CarControllerV3>().enabled = true;
                CollidedCar.GetComponent<Rigidbody>().isKinematic = false;
                CollidedCar.GetComponent<RCC_CarControllerV3>().StartEngine();

                gamePlay.trafficObject.transform.SetParent(CollidedCar.transform);
                gamePlay.trafficObject.transform.localPosition = Vector3.zero;
                gamePlay.trafficObject.transform.localRotation = Quaternion.identity;

                CollidedCar.transform.GetChild(1).gameObject.SetActive(false);

                RCC_Camera cam = RCC_SceneManager.Instance.activePlayerCamera;

                CollidedCar.AddComponent<vehiclescript>();
                Destroy(this.GetComponent<vehiclescript>());

                yield return new WaitForSeconds(2f);
                gamePlay.Button1.gameObject.SetActive(false);
                gamePlay.ExitBtn.gameObject.SetActive(false);
                ChangeEnum();


                break;

            case vehicleType.Thirdperson:
                yield return new WaitForSeconds(1f);
                gamePlay.ThirdPersonCntrol.SetActive(false);

                ChangeEnum();



                gamePlay.Button1.gameObject.SetActive(false);
                gamePlay.ExitBtn.gameObject.SetActive(true);
                Destroy(this.GetComponent<vehiclescript>());

                yield return new WaitForSeconds(2f);

                break;

            case vehicleType.Bike:
                gamePlay.BikeControl.SetActive(false);
                gamePlay.BikeCamera.SetActive(false);

                CollidedCar.GetComponent<BikeControl>().enabled = false;

                gamePlay.ThirdPersonCntrol.SetActive(false);

                CollidedCar.AddComponent<vehiclescript>();

                Destroy(this.GetComponent<vehiclescript>());

                gamePlay.Button1.gameObject.SetActive(false);
                gamePlay.ExitBike.gameObject.SetActive(true);
                break;
        }
    }
    private void ChangeEnum()
    {
        var RccScript = CollidedCar.GetComponent<RCC_CarControllerV3>();
        var BikeScript = CollidedCar.GetComponent<BikeControl>();
        var TPSScript = CollidedCar.GetComponent<BasicBehaviour>();


        if (RccScript != null)
        {
            RccScript.enabled = true;
            CollidedCar.GetComponent<Rigidbody>().isKinematic = false;
            CollidedCar.GetComponent<RCC_CarControllerV3>().StartEngine();
            gamePlay.trafficObject.transform.SetParent(CollidedCar.transform);
            gamePlay.trafficObject.transform.localPosition = Vector3.zero;
            gamePlay.trafficObject.transform.localRotation = Quaternion.identity;

            CollidedCar.transform.GetChild(1).gameObject.SetActive(false);

            foreach (GameObject carControls in gamePlay.RCCControls)
            {
                carControls.SetActive(true);
            }

            RCC_Camera cam = RCC_SceneManager.Instance.activePlayerCamera;
            CollidedCar.AddComponent<vehiclescript>();

           CollidedCar.GetComponent<vehiclescript>().currentVehicle = vehicleType.RccCars;
            
        }
        else if (BikeScript != null)
        {
            CollidedCar.GetComponent<BikeControl>().enabled = true;
            gamePlay.BikeCamera.GetComponent<BikeCamera>().target = CollidedCar.transform;
            gamePlay.BikeCamera.GetComponent<BikeCamera>().BikerMan = CollidedCar.transform.Find("Player");

            gamePlay.BikeCamera.SetActive(true);
            gamePlay.BikeControl.SetActive(true);

            CollidedCar.AddComponent<vehiclescript>();

           CollidedCar.GetComponent<vehiclescript>().currentVehicle = vehicleType.Bike;
        }
        else
        {
            currentVehicle = vehicleType.Other;
        }
    }
}