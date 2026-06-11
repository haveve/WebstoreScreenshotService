import { useEffect, useState } from "react";
import { Button, Box, Typography } from "@mui/material";

let stripePromise: any;

export const loadStripe = () => {
  if (stripePromise) return stripePromise;

  stripePromise = new Promise((resolve) => {
    if ((window as any).Stripe) {
      resolve((window as any).Stripe(process.env.REACT_APP_STRIPE_KEY));
      return;
    }

    const script = document.createElement("script");
    script.src = "https://js.stripe.com/v3/";
    script.onload = () => {
      resolve((window as any).Stripe(process.env.REACT_APP_STRIPE_KEY));
    };

    document.body.appendChild(script);
  });

  return stripePromise;
};


let stripe: any;
let elements: any;

export const StripeCheckout = ({
  amount,
  onSuccess,
}: {
  amount: number;
  onSuccess: () => void;
}) => {
  const [ready, setReady] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const init = async () => {
      stripe = await loadStripe();

      elements = stripe.elements({
        appearance: {
          theme: "stripe",
        },
      });

      const card = elements.create("card");
      card.mount("#card-element");

      card.on("change", (event: any) => {
        setError(event.error ? event.error.message : null);
      });

      setReady(true);
    };

    init();
  }, []);

  const pay = async () => {
    setLoading(true);

    try {
      const cardElement = elements.getElement("card");

      // ❗ IMPORTANT: NO PAYMENT INTENT, NO CHARGE
      const { paymentMethod, error } = await stripe.createPaymentMethod({
        type: "card",
        card: cardElement,
      });

      if (error) {
        setError(error.message);
        setLoading(false);
        return;
      }

      console.log("PaymentMethod:", paymentMethod);

      // simulate backend processing
      setTimeout(() => {
        setLoading(false);
        onSuccess();
      }, 800);
    } catch (e: any) {
      setError(e.message);
      setLoading(false);
    }
  };

  return (
    <Box>
      <Typography variant="h6" gutterBottom>
        Введіть дані картки
      </Typography>

      {/* Stripe Card UI */}
      <Box
        id="card-element"
        sx={{
          border: "1px solid #ccc",
          borderRadius: 2,
          padding: 2,
          minHeight: 50,
        }}
      />

      {error && (
        <Typography color="error" sx={{ mt: 1 }}>
          {error}
        </Typography>
      )}

      <Button
        fullWidth
        variant="contained"
        sx={{ mt: 2 }}
        disabled={!ready || loading}
        onClick={pay}
      >
        {loading ? "Оплата..." : `Заплатити $${amount}`}
      </Button>
    </Box>
  );
};