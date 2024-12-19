using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] private soCardDeck chanceDeckSource;       // ScriptableObject holding Chance cards
    [SerializeField] private soCardDeck communityChestDeckSource; // ScriptableObject holding Community Chest cards
    
    public static CardManager Instance;
    private bool actionExecuted = false;
    public iPlayer currentplayer;
    private void Awake()
    {
        Instance = this;
        InitializeDecks();
    }

    public void InitializeDecks()
    {
        if (PersistentGameData.Instance == null)
        {
            ErrorLogger.Instance.LogError("PersistentGameData.Instance is null. Ensure PersistentGameData is set up correctly.");
            return;
        }

        PersistentGameData.Instance.chanceDeck.Clear();
        PersistentGameData.Instance.communityChestDeck.Clear();

        if (chanceDeckSource == null || communityChestDeckSource == null)
        {
            ErrorLogger.Instance.LogError("ChanceDeckSource or CommunityChestDeckSource is not assigned in the CardManager.");
            return;
        }

        AddCardsToDeck(chanceDeckSource, PersistentGameData.Instance.chanceDeck);
        AddCardsToDeck(communityChestDeckSource, PersistentGameData.Instance.communityChestDeck);

        ShuffleDeck(PersistentGameData.Instance.chanceDeck);
        ShuffleDeck(PersistentGameData.Instance.communityChestDeck);

        Debug.Log($"Chance Card Count: {PersistentGameData.Instance.chanceDeck.Count}");
        Debug.Log($"Community Chest Card Count: {PersistentGameData.Instance.communityChestDeck.Count}");
    }

    private void AddCardsToDeck(soCardDeck sourceDeck, List<soCard> targetDeck)
    {
        foreach (soCard card in sourceDeck.cards)
        {
            if (card != null && card.cardType != eCardType.GOJF) // Avoid GOJF cards by default
            {
                targetDeck.Add(ScriptableObject.Instantiate(card));
                Debug.Log("Card decks initialized and shuffled.");
            }
        }
    }

    private void ShuffleDeck(List<soCard> deck)
    {
        System.Random rng = new System.Random();
        int n = deck.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            var temp = deck[k];
            deck[k] = deck[n];
            deck[n] = temp;
        }
    }
    public void DrawCard(eDeckType deckType)
    {
        List<soCard> deck = deckType == eDeckType.Chance
            ? PersistentGameData.Instance.chanceDeck
            : PersistentGameData.Instance.communityChestDeck;

        if (deck.Count == 0)
        {
            Debug.LogWarning($"The {deckType} deck is empty.");
            return;
        }

        soCard drawnCard = deck[0];
        deck.RemoveAt(0);

        if (drawnCard.cardType == eCardType.GOJF)
        {
            HandleGetOutOfJailFreeCard(deckType);
        }
        else
        {
            ShowCard(drawnCard, () =>
            {
                Debug.Log($"Card confirmed: {drawnCard.cardType}");
                ExecuteCardAction(drawnCard, PlayerManager.Instance.players[PlayerManager.Instance.curPlayer]);
            });
        }
    }
    private void ReturnCardToBottom(soCard card, GameObject cardPopup = null)
    {
        // Use the deckType field to determine the correct deck
        List<soCard> deck = card.deckType switch
        {
            eDeckType.Chance => PersistentGameData.Instance.chanceDeck,
            eDeckType.CommChest => PersistentGameData.Instance.communityChestDeck,
            _ => null
        };
        deck.Add(card);
        Debug.Log($"Returned {card.cardType} card to the bottom of the {card.deckType} deck.");
        // Destroy the card popup if provided
        Destroy(cardPopup);
    }

    private void ShowCard(soCard card, System.Action onCardConfirmed)
    {
        GameObject cardPopup = Instantiate(Resources.Load("Canvas/CanvasCard") as GameObject);
        Debug.Log("Card popup instantiated");
        WcenterCard wcenterCard = cardPopup.GetComponent<WcenterCard>();

        wcenterCard.InitWidget(card, () =>
        {
            onCardConfirmed?.Invoke();
            ReturnCardToBottom(card, cardPopup);
        });
    }
    public void ExecuteCardAction(soCard cardData, iPlayer _player)
    {
        if (actionExecuted) return; // Prevent duplicate execution
        actionExecuted = true;
        Debug.Log($"Executing card action: {cardData.actionName} for player {_player.playerName}");
        PlayerManager pm = PlayerManager.Instance;
        iPlayer currentPlayer = pm.players[pm.curPlayer];

        switch (cardData.actionName)
        {
            case "PayAll50":
                foreach (iPlayer p in pm.players)
                {
                    if (p != currentPlayer && p.so_PlayerType.playerType != ePlayerType.none)
                    {
                        p.AdjustCash(50);
                        currentPlayer.AdjustCash(-50);
                        BankManager.Instance.AddToSafeHarbor(50); // PAY TO PLAYERS
                    }
                }
                break;

            case "AdvToTerra":
                currentPlayer.MoveToTarget(ePos.HolyTerra, collect200: true);
                break;

            case "Rec50":
                currentPlayer.AdjustCash(50);
                break;

            case "Pay15":
                currentPlayer.AdjustCash(-15);
                BankManager.Instance.AddToSafeHarbor(15);
                break;

            case "AdvToStygies":
                currentPlayer.MoveToTarget(ePos.StygiesVIII, collect200: true);
                break;

            case "Rec150":
                currentPlayer.AdjustCash(150);
                break;

            case "GetOutOfJailFree":
                Debug.Log("Handled by Draw Card");
                break;

            case "GoToJail":
                Hud.Instance.GoToJail();
                break;

            case "GoBack3":
                int stepsBack = -3;
                int currentPos = (int)currentPlayer.pos;
                int newPosition = (currentPos + stepsBack) % (int)ePos.terminator;

                if (newPosition < 0)
                {
                    newPosition += (int)ePos.terminator; // Wrap around the board if negative
                }

                currentPlayer.pos = (ePos)newPosition; // Update player's logical position
                currentPlayer.MovePieceToSpot(newPosition); // Visually move the piece

                Debug.Log($"{currentPlayer.playerName} moved back 3 spaces to {Board.Instance.spots[newPosition].so_Spot.spotName}.");
                currentPlayer.SetLandingSpot(); // Trigger landing logic
                break;

            case "AdvNearestRR":
                currentPlayer.AdvanceToNearest(eSpotType.railRoad, doubleRent: true);
                break;

            case "AdvNearestUte":
                currentplayer.AdvanceToNearest(eSpotType.utility, doubleRent: false);
                break;

            case "AdvToGo":
                currentPlayer.MoveToTarget(ePos.go, collect200: true);
                break;

            case "AdvToPhalanx":
                currentPlayer.MoveToTarget(ePos.Phalanx, collect200: true);
                break;

            case "AdvToFenris":
                currentPlayer.MoveToTarget(ePos.Fenris, collect200: true);
                break;

            case "GeneralRepairs":
                int generalRepairCost = BankManager.Instance.CalculateRepairCosts(currentPlayer, 25, 100);
                currentPlayer.AdjustCash(-generalRepairCost);
                BankManager.Instance.AddToSafeHarbor(generalRepairCost);
                Debug.Log($"General Repairs cost: ${generalRepairCost}");
                break;

            case "Rec20":
                currentPlayer.AdjustCash(20);
                break;

            case "Rec200":
                currentPlayer.AdjustCash(200);
                break;

            case "Rec100":
                currentPlayer.AdjustCash(100);
                break;

            case "Pay100":
                currentPlayer.AdjustCash(-100);
                BankManager.Instance.AddToSafeHarbor(100);
                break;

            case "Pay150":
                currentPlayer.AdjustCash(-150);
                BankManager.Instance.AddToSafeHarbor(150);
                break;

            case "Pay50":
                currentPlayer.AdjustCash(-50);
                BankManager.Instance.AddToSafeHarbor(50);
                break;

            case "Rec45":
                currentPlayer.AdjustCash(45);
                break;

            case "Rec10":
                currentPlayer.AdjustCash(10);
                break;

            case "Rec25":
                currentPlayer.AdjustCash(25);
                break;

            case "Rec50FromAll":
                foreach (iPlayer p in pm.players)
                {
                    if (p != currentPlayer && p.so_PlayerType.playerType != ePlayerType.none)
                    {
                        p.AdjustCash(-50);
                        currentPlayer.AdjustCash(50);
                    }
                }
                break;

            case "StreetRepairs":
                int streetRepairCost = BankManager.Instance.CalculateRepairCosts(currentPlayer, 40, 115);
                currentPlayer.AdjustCash(-streetRepairCost);
                BankManager.Instance.AddToSafeHarbor(streetRepairCost);
                Debug.Log($"Street Repairs cost: ${streetRepairCost}");
                break;

            default:
                Debug.LogWarning($"Unknown card action: {cardData.actionName}");
                break;
        }
        actionExecuted = false; // Reset flag after execution
    }


    public void HandleGetOutOfJailFreeCard(eDeckType deckType)
    {
        List<soCard> deck = deckType == eDeckType.Chance
            ? PersistentGameData.Instance.chanceDeck
            : PersistentGameData.Instance.communityChestDeck;

        soCard gojfCard = deck.FirstOrDefault(card => card.cardType == eCardType.GOJF);
        if (gojfCard != null)
        {
            deck.Remove(gojfCard); // Remove card from deck
            currentplayer.GOJFCard++;
            Debug.Log($"{currentplayer.playerName} received a Get Out of Jail Free card from the {deckType} deck.");
        }
        else
        {
            Debug.LogWarning($"No Get Out of Jail Free cards available in the {deckType} deck.");
        }
    }
    public void UseGetOutOfJailFreeCard(iPlayer player, eDeckType deckType)
    {
        if (player.GOJFCard > 0)
        {
            player.GOJFCard--;

            List<soCard> deck = deckType == eDeckType.Chance
                ? PersistentGameData.Instance.chanceDeck
                : PersistentGameData.Instance.communityChestDeck;

            soCard gojfCard = new soCard
            {
                cardType = eCardType.GOJF,
                actionName = "GetOutOfJailFree",
                cardImage = null // Add default image if necessary
            };

            deck.Add(gojfCard); // Return the card to the deck

            Debug.Log($"{player.playerName} used a Get Out of Jail Free card and returned it to the {deckType} deck.");
        }
    }
}
