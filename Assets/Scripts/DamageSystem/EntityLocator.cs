using UnityEngine;

public class EntityLocator : MonoBehaviour
{
    
}

/*
 * Service to help track targetable objects within the scene
 *  - target selection (help enemy units find the player or NPC targets)
 *  - level progress tracking (primary level objectives, percent of enemy targets destroyed, etc.) 
 *  
 * 
 * Does this mean each entity requires a unique ID? Does Unity generate something like this for objects that I can use?
 *  - it does, but it is not persistent between scene loads. Does this use case require persistent IDs or can I use Object.GetInstanceID?
 */