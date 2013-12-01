using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Robot : MonoBehaviour
{
    public ParticleSystem psSmoke;
    public ParticleSystem psEat;

    public List<Color> colors;

    public double createTime;

    private Material mainMat;

    private double delay = 4f;

    private Transform cachedTransform;
    private GameObject huntingFood;
    private Island island;

    private Animator anim;

    private float energy;
    private float targetEnergy;

    private readonly float ENERGY_START = 0f;
    private readonly float ENERGY_PER_FOOD = 4f;

    public float[] spectrumData;
    public float[] maxes;
    public int[] activity;

    public float powerMax;
    public float power;

    public float debug;

    public float activePower
    {
        get 
        {
            if (isSilent)
                return 0f;

            float val = power / powerMax;

            if (val < 0.45f)
                return 0f;
            else
                return val;
        }
    }

    public bool isSilent
    {
        get { return powerMax < 0.01f; }
    }

    private void Awake()
    {
        spectrumData = new float[64];
        maxes = new float[64];
        activity = new int[64];

        maxes.Populate(0);
        activity.Populate(0);
    }

    private void Start()
    {
        SkinnedMeshRenderer skm =  transform.FindChild("SPIDBOT1/Cube_001").GetComponent<SkinnedMeshRenderer>();
        if (skm != null)
        {
            mainMat = skm.materials[1];

            HSBColor color = HSBColor.FromColor(colors.GetRandom());
            color.h += Random.Range(-0.05f, 0.05f);
            mainMat.color = color.ToColor();
        }

        anim = GetComponentInChildren<Animator>();
        anim.speed = 0f;

        cachedTransform = transform;

        island = transform.parent.GetComponent<Island>();

        // starting energy 
        energy = 0f;
        targetEnergy = ENERGY_START;
    }

    private void Update()
    {
        UpdateMusic();

        // update pathing
        if (island.foods.Count > 0 && huntingFood == null)
        {
            anim.speed = 0f;
            StopAllCoroutines();

            float dist = float.MaxValue;

            for (int i = 0; i < island.foods.Count; ++i)
            {
                if (island.foods[i] == null)
                    continue;

                float foodDist = Vector3.Distance(cachedTransform.position, island.foods[i].transform.position);
                if (foodDist < dist)
                {
                    huntingFood = island.foods[i];

                    foodDist = dist;
                }
            }

            if (huntingFood != null)
                StartCoroutine(HuntFood());
        }

        if (targetEnergy > energy)
        {
            energy = Mathf.Lerp(energy, targetEnergy, Time.deltaTime * 2f);

            if (energy + 0.1f > targetEnergy)
                targetEnergy = energy;
        }
        else
        {
            energy = Mathf.Max(0f, energy - Time.deltaTime);

            targetEnergy = energy;
        }

        if (energy <= 1f)
        {
            if (psSmoke.isPlaying)
                psSmoke.Stop();

            audio.volume = energy;
        }
        else
        {
            if (!psSmoke.isPlaying)
                psSmoke.Play();

            psSmoke.emissionRate = 8f + (energy / 5f);

            audio.volume = 1f;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Food f = other.transform.GetComponent<Food>();

        if (f != null)
        {
            psEat.Emit(100);
            energy += ENERGY_PER_FOOD;
            Destroy(f.gameObject);
        }
    }

    // note probably want to add values in there too, so you have a lerped value
    // going down? maybe not though. perhaps we're just always doing a value between the 
    // current value, and the actual music
    private void UpdateMusic()
    {
        power = 0f;
        powerMax = powerMax * 0.995f;

        audio.GetSpectrumData(spectrumData, 0, FFTWindow.Triangle);

        for (int i = 0; i < spectrumData.Length; ++i)
            power += spectrumData[i];

        audio.GetSpectrumData(spectrumData, 1, FFTWindow.Triangle);
        
        for (int i = 0; i < spectrumData.Length; ++i)
            power += spectrumData[i];

        powerMax = Mathf.Max(power, powerMax);

        debug = activePower;
    }

    public void PlayAtTime()
    {
        audio.PlayScheduled(createTime + delay);
    }

    private IEnumerator HuntFood()
    {
        // could mark a food item as being "hunted", if we wanted.
        Vector3 ignoreHeight = Vector3.forward + Vector3.right;

        Vector3 move = Vector3.Scale(huntingFood.transform.position - cachedTransform.position, ignoreHeight);

        Vector3 firstMove = Vector3.Scale(move, Random.value < 0.5f ? Vector3.right : Vector3.forward);
        Vector3 secondMove = move - firstMove;

        Quaternion rotTarget;
        Vector3 posTarget;

        anim.speed = 1f;

        // look in right direction.
        rotTarget = Quaternion.LookRotation(firstMove);

        while (Quaternion.Angle(cachedTransform.localRotation, rotTarget) > 1f)
        {
            cachedTransform.localRotation = Quaternion.RotateTowards(cachedTransform.localRotation, rotTarget, Time.deltaTime * 180f * 2f);
            yield return null;
        }

        cachedTransform.forward = firstMove;

        // move towards that bit.
        posTarget = Vector3.Scale(cachedTransform.position, ignoreHeight) + firstMove;

        while (Vector3.Distance(Vector3.Scale(cachedTransform.position, ignoreHeight), posTarget) > 0.25f)
        {
            cachedTransform.position += firstMove.normalized * Time.deltaTime * 5f;
            yield return null;
        }

        //cachedTransform.position = posTarget;

        // look at next direction;
        rotTarget = Quaternion.LookRotation(secondMove);

        while (Quaternion.Angle(cachedTransform.localRotation, rotTarget) > 1f)
        {
            cachedTransform.localRotation = Quaternion.RotateTowards(cachedTransform.localRotation, rotTarget, Time.deltaTime * 180f * 2f);
            yield return null;
        }

        // move towards that bit.
        posTarget = Vector3.Scale(cachedTransform.position, ignoreHeight) + secondMove;

        while (Vector3.Distance(Vector3.Scale(cachedTransform.position, ignoreHeight), posTarget) > 0.25f)
        {
            cachedTransform.position += secondMove.normalized * Time.deltaTime * 5f;
            yield return null;
        }

        //cachedTransform.position = posTarget;

        anim.speed = 0f;

        yield break;
    }


}
