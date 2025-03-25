using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIVault : UIScreen
{
    public Text wallet;
    public Text secret;
    public Text fundsXLM;
    public Text fundsTokens;
    public Text songCount;

    public Button createWalletButton;
    public Button sendFundsButton;
    public Button setFlagButton;

    


    private void Start()
    {
        createWalletButton.onClick.AddListener(()=>{
            // KeyPair keypair = StellarManager.Instance.GenerateAccountKeyPair();
            // wallet.text = keypair.AccountId;
            // secret.text = keypair.SecretSeed;

            // Debug.Log(keypair.AccountId);
            // Debug.Log(keypair.SecretSeed);
        });


        sendFundsButton.onClick.AddListener(()=>{
            // StellarManager.Instance.SendXLM();
        });

        setFlagButton.onClick.AddListener(()=>{
            // StellarManager.Instance.SetFlags();
            // StellarManager.Instance.AllowTrust();
            //StellarManager.Instance.SendAsset(StellarManager.Instance.dummyPubKey, "500");
        });
    }
}
