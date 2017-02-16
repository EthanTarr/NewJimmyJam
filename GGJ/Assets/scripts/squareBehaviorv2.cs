using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class squareBehaviorv2 : RaycastController {

    public LayerMask passengerMask;
    public Vector3 move;

    public float TotalAmplitude;
    public float Wavelength = 2f;
    public float FloorOscillation = .02f;
    public float OscillationSpeed = 0.01f;
    private float initialY = 0;
    private float standardY;

    List<PassengerMovement> passengerMovement;
    Dictionary<Transform, Controller2D> PassengerDictionary = new Dictionary<Transform, Controller2D>();

    [HideInInspector]
    public bool firstBlock;

    // Use this for initialization
    Vector2 lastPosition;
    float velocity;
    public float deltaVelocity = 0;
    float lastVelocity = 0;

    public override void Start() {
        lastPosition = transform.position;
        standardY = transform.position.y;
        base.Start();
    }

    void Update() {
        initialY = transform.position.y;
        TotalAmplitude = 0;
        standardY += FloorOscillation * (Mathf.Sin(Time.time * OscillationSpeed));

        foreach (GameObject pulse in GameObject.FindGameObjectsWithTag("Pulse"))
        {
            float xPos = transform.position.x;
            float xPulsePos = pulse.transform.position.x;

            if (xPos - xPulsePos < Wavelength && xPos - xPulsePos > -Wavelength)
            {
                TotalAmplitude += pulse.GetComponent<PulseMove>().Amplitude * Mathf.Sin(((Mathf.PI / Wavelength) * (xPos - xPulsePos)));
            }
        }
        foreach (GameObject pulse in GameObject.FindGameObjectsWithTag("AntiPulse"))
        {
            float xPos = transform.position.x;
            float xPulsePos = pulse.transform.position.x;

            if (xPos - xPulsePos < Wavelength && xPos - xPulsePos > -Wavelength) {
                TotalAmplitude += -pulse.GetComponent<AntiPulseMove>().Amplitude * Mathf.Sin((Mathf.PI / Wavelength) * (xPos - xPulsePos));
            }
        }
        TotalAmplitude = Mathf.Clamp(TotalAmplitude, -10, 10);

        transform.position = new Vector3(transform.position.x, Mathf.Lerp(initialY, TotalAmplitude + standardY, Time.deltaTime), 0);

        if (firstBlock) {
            GetComponent<SpriteRenderer>().color = Color.Lerp(GetComponent<SpriteRenderer>().color, Color.white, Time.deltaTime);
        }

        getVelocity();
        UpdateRaycastOrigins();
        CalculatePassengerMovement(new Vector3(0, velocity));


        MovePassengers(true);
        //transform.Translate(new Vector3(0, velocity));
        //MovePassengers(false);
    }

    void MovePassengers(bool beforeMovePlatfrom) {
        foreach (PassengerMovement passenger in passengerMovement) {
            if (!PassengerDictionary.ContainsKey(passenger.transforms)) {
                PassengerDictionary.Add(passenger.transforms, passenger.transforms.GetComponent<Controller2D>());
            }

            if (passenger.moveBeforePlatform == beforeMovePlatfrom) {
                PassengerDictionary[passenger.transforms].Move(passenger.velocity, passenger.standingOnPlatform);
                
            } 
        }
    }

    void CalculatePassengerMovement(Vector3 velocity) {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovement = new List<PassengerMovement>();
        float directionY = Mathf.Sign(velocity.y);

        // Vertically moving platform
        if (velocity.y != 0) {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < VerticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigin.bottomLeft : raycastOrigin.topRight;
                rayOrigin += (Vector2)(-directionY * transform.right) * (VecticalRaySpacing * i + velocity.x);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);
                if (hit) {
                    if (!movedPassengers.Contains(hit.transform)) {
                        movedPassengers.Add(hit.transform);
                        float pushX = (directionY == 1) ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                    }
                }
            }
        }

        // Passenger on top of a horizontally or downward moving platform
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLength = skinWidth * 2;

            for (int i = 0; i < VerticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigin.topLeft + Vector2.right * (VerticalRayCount * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y;

                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                    }
                }
            }
        }
    }

    void getVelocity() {
        velocity = transform.position.y - lastPosition.y;
        lastPosition = transform.position;
        getDeltaVelocity();
    }


    public float getDeltaVelocity() {
        deltaVelocity = velocity - lastVelocity;
        lastVelocity = velocity;
        return deltaVelocity;
    }

    struct PassengerMovement{
        public Transform transforms;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform _transforms, Vector3 _velocity, bool _standingOnPlatform, bool _movePlatform) {
            transforms = _transforms;
            velocity = _velocity;
            standingOnPlatform = _standingOnPlatform;
            moveBeforePlatform = _movePlatform;
        }
    }
        
}

