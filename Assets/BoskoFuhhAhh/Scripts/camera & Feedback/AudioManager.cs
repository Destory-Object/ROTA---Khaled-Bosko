using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        public bool randomizePitch = true; // stops repeated hits sound
        [Min(0)] public float delay = 0f;
    }

    [SerializeField] private Sound[] sounds;
    [SerializeField] private int poolSize = 8;

    private static readonly string[] defaultSoundNames =
    {
        "SwordSwing",
        "CritHit",
        "DaggerSwing",
        "HeavySwordSwing",
        "HitImpact",
        "ChargeHit",
        "ShotgunFire",
        "ParrySuccess",
        "Jump",
        "DoubleJump",
        "Bounce",
        "EnemyDeath",
        "Pickup",
    };

    private Dictionary<string, Sound> soundDict;
    private List<AudioSource> pool = new List<AudioSource>();

    private void Awake()
    {
        Instance = this;

        soundDict = new Dictionary<string, Sound>();
        foreach (var s in sounds)
            soundDict[s.name] = s;

        for (int i = 0; i < poolSize; i++)
            pool.Add(gameObject.AddComponent<AudioSource>());
    }

    public static void Play(string name)
    {
        Instance?.PlayInternal(name);
    }

    private void PlayInternal(string name)
    {
        if (!soundDict.TryGetValue(name, out Sound s) || s.clip == null)
        {
            // No warning spam for clips you haven't assigned yet — just skip.
            return;
        }

        AudioSource src = GetFreeSource();
        src.clip = s.clip;
        src.volume = s.volume;
        src.pitch = s.randomizePitch ? Random.Range(0.92f, 1.08f) : 1f;
        if (s.delay > 0)
            src.PlayDelayed(s.delay);
        else
            src.Play();
    }

    private AudioSource GetFreeSource()
    {
        foreach (var src in pool)
            if (!src.isPlaying) return src;

        // every source busy (rare) — steal the first one rather than dropping the sound
        return pool[0];
    }

#if UNITY_EDITOR
    [ContextMenu("Populate Sound Names")]
    private void PopulateSoundNames()
    {
        List<Sound> existing = sounds != null ? new List<Sound>(sounds) : new List<Sound>();
        HashSet<string> haveNames = new HashSet<string>();
        foreach (var s in existing)
            haveNames.Add(s.name);

        foreach (string name in defaultSoundNames)
        {
            if (!haveNames.Contains(name))
                existing.Add(new Sound { name = name, volume = 1f, randomizePitch = true });
        }

        sounds = existing.ToArray();
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"AudioManager: populated {defaultSoundNames.Length} sound slots. Drag your clips onto them in the Inspector.");
    }
#endif
}