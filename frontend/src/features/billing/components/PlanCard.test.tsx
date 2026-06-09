import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { PlanCard } from "./PlanCard";
import type { Plan } from "../../conversions/types/subscriptionTypes";

const freePlan: Plan = {
  id: "free-id",
  name: "Free",
  slug: "free",
  monthlyConversionLimit: 5,
  maxFileSizeMb: 10,
  retentionHours: 24,
  priceCents: 0,
  isActive: true,
};

describe("PlanCard", () => {
  it("marks the current plan", () => {
    render(<PlanCard isChanging={false} isCurrent onChangePlan={vi.fn()} plan={freePlan} />);

    expect(screen.getByText("Free")).toBeInTheDocument();
    expect(screen.getByText("Current plan")).toBeInTheDocument();
  });
});
