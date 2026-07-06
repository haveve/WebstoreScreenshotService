import { createSlice, PayloadAction } from "@reduxjs/toolkit";

export type BasketItem = {
  id: string;
  type: "points";
  amount: number;
  price: number;
};

type BasketState = {
  items: BasketItem[];
  open: boolean;
};

const initialState: BasketState = {
  items: [],
  open: false,
};

export const basketSlice = createSlice({
  name: "basket",
  initialState,
  reducers: {
    addPoints: (state, action: PayloadAction<{ amount: number; price: number }>) => {
      const pointsItem = state.items.find(i => i.type === "points");

      if(pointsItem){
        pointsItem.amount += action.payload.amount;
        pointsItem.price = getPrice(pointsItem.amount);
        return;
      }

      state.items.push({
        id: crypto.randomUUID(),
        type: "points",
        amount: action.payload.amount,
        price: action.payload.price,
      });
    },

    removeItem: (state, action: PayloadAction<string>) => {
      state.items = state.items.filter(i => i.id !== action.payload);
    },

    clearBasket: (state) => {
      state.items = [];
    },

    toggleBasket: (state) => {
      state.open = !state.open;
    },
  },
});

const POINTS_PRICE_TIERS = [
  { upTo: 5000, pricePer1000: 1.99 },
  { upTo: 100_000, pricePer1000: 1.79 },
  { upTo: 200_000, pricePer1000: 1.59 },
  { upTo: Infinity, pricePer1000: 1.39 },
];

export const getPrice = (points: number) => {
  let remaining = points;
  let total = 0;

  let prevLimit = 0;

  for (const tier of POINTS_PRICE_TIERS) {
    const tierRange = tier.upTo - prevLimit;
    const applicable = Math.min(remaining, tierRange);

    if (applicable <= 0) break;

    total += (applicable / 1000) * tier.pricePer1000;

    remaining -= applicable;
    prevLimit = tier.upTo;
  }

  return Number(total.toFixed(2));
};

export const { addPoints, removeItem, clearBasket, toggleBasket } = basketSlice.actions;
export default basketSlice.reducer;