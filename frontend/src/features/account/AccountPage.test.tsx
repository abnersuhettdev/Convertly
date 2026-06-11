import { screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi } from "vitest";
import { renderWithAppProviders } from "../../test/test-utils";
import { AccountPage } from "./AccountPage";

vi.mock("../auth/hooks/useAuth", () => ({
  useAuth: () => ({
    logout: vi.fn(),
    user: {
      id: "user-1",
      name: "Abner Suhett",
      email: "abner@example.com",
    },
  }),
}));

vi.mock("../conversions/hooks/useSubscription", () => ({
  useSubscription: () => ({
    data: {
      plan: { name: "Free" },
      monthlyLimit: 5,
      conversionsUsed: 1,
      conversionsRemaining: 4,
      retentionHours: 24,
    },
    isError: false,
    isLoading: false,
  }),
}));

vi.mock("./services/accountService", () => ({
  changePassword: vi.fn(),
  deleteAccount: vi.fn(),
  getAccountErrorMessage: vi.fn(() => "Request failed."),
}));

describe("AccountPage", () => {
  it("validates password confirmation before submitting", async () => {
    const user = userEvent.setup();
    renderWithAppProviders(<AccountPage />);

    await user.type(screen.getAllByLabelText("Current password")[0], "StrongPassword123!");
    await user.type(screen.getByLabelText("New password"), "NewStrongPassword123!");
    await user.type(screen.getByLabelText("Confirm new password"), "DifferentPassword123!");
    await user.click(screen.getByRole("button", { name: "Change password" }));

    expect(screen.getByText("Password confirmation must match the new password.")).toBeInTheDocument();
  });

  it("keeps delete account disabled until explicit confirmation", async () => {
    const user = userEvent.setup();
    renderWithAppProviders(<AccountPage />);

    const deleteButton = screen.getByRole("button", { name: "Delete account" });
    expect(deleteButton).toBeDisabled();

    await user.click(screen.getByLabelText("I understand this action will disable my account and sign me out."));

    expect(deleteButton).toBeEnabled();
  });
});
