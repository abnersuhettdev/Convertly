export type Plan = {
  id: string;
  name: string;
  slug: string;
  monthlyConversionLimit: number;
  maxFileSizeMb: number;
  retentionHours: number;
  priceCents: number;
  isActive: boolean;
};

export type Subscription = {
  plan: Plan;
  monthlyLimit: number;
  conversionsUsed: number;
  conversionsRemaining: number;
  maxFileSizeMb: number;
  retentionHours: number;
};
