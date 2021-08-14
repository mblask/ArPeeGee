using UnityEngine;

[CreateAssetMenu(fileName = "New Book/Scroll", menuName = "ArPeeGee/Items/BookScroll", order = 4)]
public class BookScroll : Item
{
    public GameObject SpellToLearn;
    public AudioClip BookScroolAudioClip;

    public override void Use()
    {
        if (SpellToLearn == null)
        {
            Debug.LogWarning("No spell attached to scroll");
            return;
        }

        Spell spell = SpellToLearn.GetComponent<Spell>();

        AudioManager.Instance.SFXAudioSource.PlayOneShot(BookScroolAudioClip);
        if (PlayerController.Instance.LearnSpell(spell))
            RemoveFromInventory();
    }
}
