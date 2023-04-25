using UnityEngine;

public enum CollectibleType
{
    Score,
    Time
}

public class CollectibleBehavior : MonoBehaviour
{
    public CollectibleType collectibleType;
    public float addedTime = 30f; // The amount of time added when collecting a Time Collectible

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter called");
        if (other.CompareTag("MainCamera"))
        {
            if (collectibleType == CollectibleType.Score)
            {
                // Increment the score.
                CollectibleManager.instance.IncrementScore();
            }
            else if (collectibleType == CollectibleType.Time)
            {
                // Add time to the user's playtime.
                CollectibleManager.instance.AddTime(addedTime);
            }

            // Destroy the collectible.
            Destroy(gameObject);
        }
    }
}
