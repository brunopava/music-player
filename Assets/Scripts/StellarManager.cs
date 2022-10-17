using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses;
using stellar_dotnet_sdk.requests;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UStellar.Core;

using UnityEngine.UI;

public class StellarManager : Singleton<StellarManager>
{
    private string issuerPubKey = "GCCC7OZWABG3TULA37UYOKCRAUFNQXGFRDVUGVUQ5CNUZH5EK4PBJW7W";
    private string issuerPrivKey = "SCBQEETB66LPFESOKYEF7EHLFZCPWM3ZOXTT6EKG7EZVFGGZFTVFSBAQ";

    private string distributorPubKey = "GC6NMCCRSDIHLJQM4VFMFZ2WBRDV456U7VAGC22ABG7B2W7A2D5E7TKZ";
    private string distributorPrivKey = "SDZWBIPOI6YZL35TYTW7AWC624MWIPS2VZLUGE5WZCGRRKHDLNKBIAJH";

    public string dummyPubKey = "GDR77Y7ND6R3VNZC5KIWTJ7VAKXVXY47VIDZH5RSDQBA6HNF5AJAUE6T";
    private string dummyPrivKey = "SCKUXQNPRYEJUQW6THRFJKEHDSGLUQG7QP4DRZNTHCUF6URIYL5YWNRU";

    private void Start()
    {
        UStellar.Core.UStellarManager.SetStellarTestNetwork();
        UStellar.Core.UStellarManager.Init();
    }

    public KeyPair GenerateAccountKeyPair()
    {
        KeyPair newKeyPair = KeyPair.Random();
        return newKeyPair;
    }

    public async void SetFlags()
    {
        /**
        * Lock our asset by setting an option
        * AUTHORIZATION REQUIRED flag
        * to issuer account
        */
        Server server = UStellarManager.GetServer();

        await server.Accounts.Account(issuerPubKey);
        AccountResponse sourceAccountResponse = await server.Accounts.Account(issuerPubKey);
        Account account = new Account(sourceAccountResponse.AccountId, sourceAccountResponse.SequenceNumber);

        KeyPair issuerKey = KeyPair.FromSecretSeed(issuerPrivKey);

        string ASSET_NAME = "VNM";
        Asset asset = Asset.CreateNonNativeAsset(ASSET_NAME, issuerPubKey);

        stellar_dotnet_sdk.xdr.SetOptionsOp setFlagOp = new stellar_dotnet_sdk.xdr.SetOptionsOp();
        uint value = 0x1;
        setFlagOp.SetFlags = new stellar_dotnet_sdk.xdr.Uint32(value);

        SetOptionsOperation operation = new SetOptionsOperation.Builder(setFlagOp)
                                                               .SetSourceAccount(issuerKey)
                                                               .Build();

        Transaction transaction = new TransactionBuilder(account)
                                                        .AddOperation(operation)
                                                        .Build();

        transaction.Sign(KeyPair.FromSecretSeed(issuerPrivKey));

        try
        {
            await server.SubmitTransaction(transaction);
            Debug.Log("Success!");
        }
        catch (Exception exception)
        {
            Debug.Log(exception);
        }
    }

    public async void ChangeDistributorTrust()
    {
        /**
         * Change distributor trustline to trust
         * issuer
         */
        Server server = UStellarManager.GetServer();

        await server.Accounts.Account(distributorPubKey);
        AccountResponse sourceAccountResponse = await server.Accounts.Account(distributorPubKey);
        Account account = new Account(sourceAccountResponse.AccountId, sourceAccountResponse.SequenceNumber);

        string ASSET_NAME = "VNM";
        Asset asset = Asset.CreateNonNativeAsset(ASSET_NAME, issuerPubKey);

        stellar_dotnet_sdk.xdr.ChangeTrustOp changeTrustAsset = new stellar_dotnet_sdk.xdr.ChangeTrustOp();
        stellar_dotnet_sdk.xdr.Int64 limit = new stellar_dotnet_sdk.xdr.Int64();
        limit.InnerValue = 1000000000;

        changeTrustAsset.Line = asset.ToXdr();
        changeTrustAsset.Limit = limit;

        ChangeTrustOperation operation = new ChangeTrustOperation.Builder(changeTrustAsset)
                                                                 .Build();

        Transaction transaction = new TransactionBuilder(account).AddOperation(operation)
                                                                 .Build();

        transaction.Sign(KeyPair.FromSecretSeed(distributorPrivKey));

        try
        {
            await server.SubmitTransaction(transaction);
            Debug.Log("Distributor changed trustline to issuer");
        }
        catch (Exception exception)
        {
            Debug.Log(exception);
        }
    }

    public async void IssueAsset()
    {
        Server server = UStellarManager.GetServer();

        KeyPair issuerKeypair = KeyPair.FromSecretSeed(issuerPrivKey);

        //Check if the destination account exists in the server.
        await server.Accounts.Account(distributorPubKey);

        //Load up to date information in source account
        await server.Accounts.Account(issuerKeypair.AccountId);

        AccountResponse issuerAccountResponse = await server.Accounts.Account(issuerKeypair.AccountId);
        Account issuerAccount = new Account(issuerAccountResponse.AccountId, issuerAccountResponse.SequenceNumber);


        string ASSET_NAME = "VNM";
        string amount = "200";
        Asset asset = Asset.CreateNonNativeAsset(ASSET_NAME, issuerPubKey);


        PaymentOperation operation = new PaymentOperation.Builder(KeyPair.FromAccountId(distributorPubKey), asset, amount)
                                                         .SetSourceAccount(issuerAccount.KeyPair)
                                                         .Build();

        Transaction transaction = new TransactionBuilder(issuerAccount)
                                                                    .AddOperation(operation)
                                                                    .Build();

        Debug.Log("Signing");

        transaction.Sign(KeyPair.FromSecretSeed(issuerPrivKey));

        Debug.Log("COMPLETE");

        try
        {
            await server.SubmitTransaction(transaction);
            Debug.Log("Success!");
        }
        catch (Exception exception)
        {
            Debug.Log(exception);

            // If the result is unknown (no response body, timeout etc.) we simply resubmit
            // already built transaction:
            // SubmitTransactionResponse response = server.submitTransaction(transaction);
        }
    }

    public async void AllowTrust(string target)
    {
        /**
         * Change distributor trustline to trust
         * issuer
         */
        Server server = UStellarManager.GetServer();

        KeyPair trustor = KeyPair.FromAccountId(target);

        await server.Accounts.Account(issuerPubKey);
        AccountResponse sourceAccountResponse = await server.Accounts.Account(issuerPubKey);
        Account account = new Account(sourceAccountResponse.AccountId, sourceAccountResponse.SequenceNumber);

        string ASSET_NAME = "VNM";

        AllowTrustOperation operation = new AllowTrustOperation.Builder(trustor, ASSET_NAME, true, false)
                                                               .SetSourceAccount(KeyPair.FromAccountId(issuerPubKey))
                                                                 .Build();

        Transaction transaction = new TransactionBuilder(account).AddOperation(operation)
                                                                 .Build();

        Debug.Log("Signing...");
        transaction.Sign(KeyPair.FromSecretSeed(issuerPrivKey));
        Debug.Log("COMPLETE");

        try
        {
            await server.SubmitTransaction(transaction);
            Debug.Log("Allow trust complete");
        }
        catch (Exception exception)
        {
            Debug.Log(exception);
        }
    }


    public async void SendAsset(string targetWallet, string amount)
    {
        Server server = UStellarManager.GetServer();

        KeyPair distributorKeyPair = KeyPair.FromSecretSeed(distributorPrivKey);

        await server.Accounts.Account(distributorPubKey);
        await server.Accounts.Account(distributorKeyPair.AccountId);

        AccountResponse distAccResp = await server.Accounts.Account(distributorKeyPair.AccountId);
        Account distributorAccount = new Account(distAccResp.AccountId, distAccResp.SequenceNumber);

        string ASSET_NAME = "VNM";
        Asset asset = Asset.CreateNonNativeAsset(ASSET_NAME, issuerPubKey);


        PaymentOperation operation = new PaymentOperation.Builder(KeyPair.FromAccountId(targetWallet), asset, amount)
                                                         .SetSourceAccount(distributorAccount.KeyPair)
                                                         .Build();

        Transaction transaction = new TransactionBuilder(distributorAccount)
                                                                    .AddOperation(operation)
                                                                    .Build();

        Debug.Log("Signing");
        transaction.Sign(KeyPair.FromSecretSeed(distributorPrivKey));
        Debug.Log("COMPLETE");

        try
        {
            await server.SubmitTransaction(transaction);
            Debug.Log("Success!");
        }
        catch (Exception exception)
        {
            Debug.Log(exception);
        }
    }

    // private void AllowTrust()
    // {
    //     /**
    //      * Allow distributor to hold
    //      * our custom asset
    //      */

    //     const allowTrustTransacton = new StellarSdk.TransactionBuilder(issuer)
    //       .addOperation(
    //         StellarSdk.Operation.allowTrust({
    //           assetCode: ASSET_NAME,
    //           authorize: true,
    //           trustor: distributor.id
    //         })
    //       )
    //       .build()
    //     allowTrustTransacton.sign(issuerKey)

    //     await server.submitTransaction(allowTrustTransacton)
    //     console.log('[main] issuer allow trust for distributor')
    // }


    // private void IssueAsset()
    // {
    //     /**
    //      * Issuing Asset to distributor
    //      */

    //     const issuingAssetToDistributorTransaction = new StellarSdk.TransactionBuilder(
    //       issuer
    //     )
    //       .addOperation(
    //         StellarSdk.Operation.payment({
    //           asset: AppAsset,
    //           amount: '300',
    //           destination: distributorKey.publicKey()
    //         })
    //       )
    //       .build()
    //     issuingAssetToDistributorTransaction.sign(issuerKey)

    //     await server.submitTransaction(issuingAssetToDistributorTransaction)

    //     distributor.balances.forEach((asset) => {
    //       console.log(asset)
    //     })
    // }

    // private void Customer01()
    // {
    //     /**
    //      * Customer init wallet and account
    //      * and then try to add trustline without
    //      * authorization
    //      */

    //     const wallet = await createWalllet(server)

    //     if (!wallet) {
    //       throw new Error('[main] wallet is unavaliable')
    //     }
    //     const walletChangeTrustTransaction = new StellarSdk.TransactionBuilder(
    //       wallet
    //     )
    //       .addOperation(
    //         StellarSdk.Operation.changeTrust({
    //           asset: AppAsset
    //         })
    //       )
    //       .build()

    //     const walletKey = await getWalletKey()
    //     walletChangeTrustTransaction.sign(walletKey)

    //     try {
    //       await server.submitTransaction(walletChangeTrustTransaction)
    //       console.log('[main] wallet change trust done')
    //     } catch (e) {
    //       console.error(e)
    //     }
    // }


    // private void PayCustomer()
    // {
    //     /**
    //      * Try to send asset to customer
    //      * Distributor will failed at the first try to send
    //      * some asset to wallet
    //      */

    //     const distributorPaymentToCustomerTransaction = new StellarSdk.TransactionBuilder(
    //       distributor
    //     )
    //       .addOperation(
    //         StellarSdk.Operation.payment({
    //           asset: AppAsset,
    //           amount: '20',
    //           destination: walletKey.publicKey()
    //         })
    //       )
    //       .build()

    //     distributorPaymentToCustomerTransaction.sign(distributorKey)

    //     try {
    //       await server.submitTransaction(distributorPaymentToCustomerTransaction)
    //     } catch (e) {
    //       // transaction will failed
    //       // here resp with 400 operation
    //       console.log(e)
    //     }
    // }

    // private void AllowWalletToHold()
    // {
    //     /**
    //      * To allow wallet to hold our asset
    //      * issuer should
    //      * AllowTrust for customer wallet key
    //      */

    //     const issuerAllowTrustWalletKeyTransaction = new StellarSdk.TransactionBuilder(
    //     issuer)
    //     .addOperation(
    //       StellarSdk.Operation.allowTrust({
    //         assetCode: AppAsset.code,
    //         authorize: true,
    //         trustor: wallet.id
    //       })
    //     )
    //     .build()

    //     issuerAllowTrustWalletKeyTransaction.sign(issuerKey)

    //     await server.submitTransaction(issuerAllowTrustWalletKeyTransaction)
    //     console.log(`[main] issuer allow trust for key ${wallet.id} done `)

    //     // and then ask distributor to
    //     // try to send Asset to wallet again
    //     const distributorSecondAttemptPaymentToCustomerTransaction = new StellarSdk.TransactionBuilder(
    //       distributor
    //     )
    //       .addOperation(
    //         StellarSdk.Operation.payment({
    //           asset: AppAsset,
    //           amount: '20',
    //           destination: walletKey.publicKey()
    //         })
    //       )
    //       .build()

    //     distributorSecondAttemptPaymentToCustomerTransaction.sign(distributorKey)
    //     await server.submitTransaction(distributorSecondAttemptPaymentToCustomerTransaction)

    //     console.log('[main] customer already receive asset')
    // }
}
