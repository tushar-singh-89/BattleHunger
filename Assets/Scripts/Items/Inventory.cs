﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Random = UnityEngine.Random;
using System;

public class Inventory : MonoBehaviour
{
	public static Inventory instance;
    public int numberOfSlots = 4;
    public GameObject slotHolder;
    public int slotSelected = 0;
    private InputComponent input;
    public Slot[] itemSlots;
    private SoundManager soundManager;

	#region Singleton
	void Awake()
	{
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    #endregion

    public GameObject[] slots;

    public void Start()
    {
        soundManager = SoundManager.instance;
        input = GetComponent<InputComponent>();

        input.OnInventoryMoveLeft += ChangeSlotLeft;
        input.OnInventoryMoveRight += ChangeSlotRight;
        input.OnUseInventoryItem += UseItemSelected;
        input.OnInventoryDropItem += DropItemSelected;

        slots = new GameObject[numberOfSlots];

        for (int i = 0; i < numberOfSlots; i++)
        {
           slots[i] = slotHolder.transform.GetChild(i).gameObject;

            if (slots[i].GetComponent<Slot>().itemName == "")
                slots[i].GetComponent<Slot>().isEmpty = true;

        }

        // Get each slot in the Inventories assigned Grid
        itemSlots = slotHolder.GetComponentsInChildren<Slot>();

    }

    /*
     * Used when loading to populate the inventory based on what the player should have/had before loading
     */ 
    public void PopulateInventory()
    {
        foreach (Slot s in itemSlots)
        {
            foreach (GameObject item in GameManagerSingleton.instance.GetComponent<ItemDatabase>().items)
            {
                if (s.itemID.Equals(item.GetComponent<Item>().itemID))
                    s.item = item.GetComponent<Item>();
            }
        }
    }

    /*
     * Set the highlight image thats currently active (if there is one) inactive, and activate the next one
     */
    void ChangeSlotRight()
    {
        if (slotSelected < numberOfSlots - 1)
        {
            if (itemSlots[slotSelected].GetComponentInChildren<RectTransform>().GetChild(0).gameObject.activeSelf)
                itemSlots[slotSelected].GetComponentInChildren<RectTransform>().GetChild(0).gameObject.SetActive(false);

            slotSelected++;
            itemSlots[slotSelected].GetComponentInChildren<RectTransform>().GetChild(0).gameObject.SetActive(true);
            if (slots[slotSelected].GetComponent<Slot>().item != null)
                slots[slotSelected].GetComponent<Slot>().DisplayItemInfo();
            else
                slots[slotSelected].GetComponent<Slot>().DisableItemInfo();
        }
    }

    /*
    * Set the highlight image thats currently active (if there is one) inactive, and activate the next one
    */
    void ChangeSlotLeft()
    {
        if (slotSelected > 0)
        {
            if (itemSlots[slotSelected].GetComponentInChildren<RectTransform>().GetChild(0).gameObject.activeSelf)
                itemSlots[slotSelected].GetComponentInChildren<RectTransform>().GetChild(0).gameObject.SetActive(false);

            slotSelected--;
            itemSlots[slotSelected].GetComponentInChildren<RectTransform>().GetChild(0).gameObject.SetActive(true);
            if (slots[slotSelected].GetComponent<Slot>().item != null)
                slots[slotSelected].GetComponent<Slot>().DisplayItemInfo();
            else
                slots[slotSelected].GetComponent<Slot>().DisableItemInfo();
        }
    }

    /*
     * Helper to use an item for controller
     */ 
    void UseItemSelected()
    {
        if (!itemSlots[slotSelected].isEmpty)
            itemSlots[slotSelected].UseItem();
    }

    /*
     * Helper for controller to drop an item
     */ 
    void DropItemSelected()
    {
        if(Time.timeScale != 0)
        {
            if (!itemSlots[slotSelected].isEmpty)
            {
                itemSlots[slotSelected].RemoveItem();
            }
        }
    }

    /*
     * Add an item to the player's inventory
     * Set the Slot variables to the item's variables and play a random sound.
     * Increment the numberOfNotesFound variables from NoteHandler if item is a Note
     */
    public void Add(Item item)
	{
        for (int i = 0; i < numberOfSlots; i++)
        {
            if(slots[i].GetComponent<Slot>().isEmpty) //empty slot
            {
                slots[i].GetComponent<Slot>().item = item;
                slots[i].GetComponent<Slot>().itemName = item.itemName;
                slots[i].GetComponent<Slot>().description = item.description;
                slots[i].GetComponent<Slot>().icon = item.icon;
                slots[i].GetComponent<Slot>().itemID = item.itemID;
                slots[i].GetComponent<Slot>().isEmpty = false;
                item.gameObject.SetActive(false);
                GameManagerSingleton.instance.player.GetComponent<InteractComponent>().RemoveFocus();

                // Play random pickup sound
                soundManager.PlayRandomOneShot(soundManager.pickUpItemSounds);

                if (item is NoteItem)
                    GameManagerSingleton.instance.GetComponent<NoteHandler>().numberOfNotesFound++; // increment number of notes

                return;
            }
        }
	}

    /*
     * Remove an item from the inventory by checking if the item's ID equals an item's ID in the database.
     * If the item is a weapon, then unequip it.
     * If the item is a note, destroy it, else if it's not, drop it on the ground.
     * Also, player a random sound
     */ 
    public void Remove(Item item)
	{
        if (item != null)
        {
            foreach(GameObject i in GameManagerSingleton.instance.GetComponent<ItemDatabase>().items)
            {
                if(item.itemID.Equals(i.GetComponent<Item>().itemID))
                {
                    // If the weapon item id is the same as the equipped weapon, unequip first
                    if (GetComponentInChildren<PlayerWeapon>().equippedWeapon != null)
                    {
                        if (item.itemID == GetComponentInChildren<PlayerWeapon>().equippedWeapon.itemID)
                        {
                            WeaponItem weapon = (WeaponItem)item;
                            GetComponentInChildren<PlayerWeapon>().UnequipWeapon(weapon);
                        }
                    }

                    if(item is NoteItem)
                    {
                        GameManagerSingleton.instance.GetComponent<NoteHandler>().isNotePanelOpen = false;
                    }
                    else
                        Instantiate(i, gameObject.transform.position, Quaternion.identity);

                    // Play random drop sound
                    soundManager.PlayRandomOneShot(soundManager.dropItemSounds);
                }
            }
        }
	}

	public void ListItems()
	{
	}
}
