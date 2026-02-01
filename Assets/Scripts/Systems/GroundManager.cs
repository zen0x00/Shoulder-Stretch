using UnityEngine;
using System.Collections.Generic;
public class GroundManager : MonoBehaviour
{
  [SerializeField] private PlayerController player;
  [SerializeField] private GameObject groundPrefab;
  [SerializeField] private int poolSize = 5;
  [SerializeField] private float segmentLength = 20f;
  [SerializeField] private float spawnDistance = 60f;
  private List<GameObject> activeSegments = new List<GameObject>();
  private float nextSpawnZ;
  private void Start()
  {
    if (player == null) player = FindFirstObjectByType<PlayerController>();
    for (int i = 0; i < poolSize; i++) SpawnSegment();
  }
  private void Update()
  {
    if (player == null) return;
    if (player.transform.position.z + spawnDistance > nextSpawnZ) SpawnSegment();
    if (activeSegments.Count > poolSize)
    {
      GameObject old = activeSegments[0];
      if (player.transform.position.z > old.transform.position.z + segmentLength)
      {
        activeSegments.RemoveAt(0);
        RecycleSegment(old);
      }
    }
  }
  private void SpawnSegment()
  {
    GameObject segment = Instantiate(groundPrefab, new Vector3(0, -0.5f, nextSpawnZ), Quaternion.identity, transform);
    ApplyProceduralVariation(segment);
    activeSegments.Add(segment);
    nextSpawnZ += segmentLength;
  }
  private void RecycleSegment(GameObject segment)
  {
    segment.transform.position = new Vector3(0, -0.5f, nextSpawnZ);
    ApplyProceduralVariation(segment);
    activeSegments.Add(segment);
    nextSpawnZ += segmentLength;
  }
  private void ApplyProceduralVariation(GameObject segment)
  {
    float randomWidth = Random.Range(10f, 15f);
    segment.transform.localScale = new Vector3(randomWidth, 1f, segmentLength);
    foreach (Transform child in segment.transform) if (child.name.Contains("Marker")) child.gameObject.SetActive(Random.value > 0.5f);
  }
}
