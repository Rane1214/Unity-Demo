using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAP : MonoBehaviour
{
    IAP_Base Base;

    private void Awake()
    {
        Base = new IAP_Base();
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        Base.OnPurchaseFailed(i, p);
    }

    public void OnPurchaseComplete(Product product)
    {
        Base.OnPurchaseComplete(product);
    }
}


public class IAP_Base : IStoreListener
{
    private IStoreController controller;
    private IExtensionProvider extensions;
    private static string IAPID_AdsRemove = "adsremove.simplemerge2";

    public IAP_Base()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(IAPID_AdsRemove, ProductType.Consumable, new IDs
        {
            {IAPID_AdsRemove, GooglePlay.Name},
            {IAPID_AdsRemove, MacAppStore.Name}
        });

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;
        this.extensions = extensions;

        Game_Manager.AdsRemoved(AdMober.AdsRemoved = controller.products.WithID(IAPID_AdsRemove).hasReceipt);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {

    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {

    }

    public void OnPurchaseComplete(Product product)
    {
        if (product.definition.id == IAPID_AdsRemove)
            Game_Manager.AdsRemoved(AdMober.AdsRemoved = true);
    }
}
