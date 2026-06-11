import { screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi } from "vitest";
import { renderWithAppProviders } from "../../../test/test-utils";
import { NewConversionPage } from "./NewConversionPage";

vi.mock("../hooks/useSubscription", () => ({
  useSubscription: () => ({
    data: {
      plan: { name: "Free" },
      maxFileSizeMb: 10,
    },
    error: null,
    isError: false,
    isLoading: false,
  }),
}));

vi.mock("../hooks/useCreateConversion", () => ({
  useCreateConversion: () => ({
    error: null,
    isPending: false,
    mutateAsync: vi.fn(),
  }),
}));

describe("NewConversionPage", () => {
  it("requires content responsibility confirmation before creating a conversion", async () => {
    const user = userEvent.setup();
    renderWithAppProviders(<NewConversionPage />);

    const createButton = screen.getByRole("button", { name: "Create conversion" });
    expect(createButton).toBeDisabled();
    expect(createButton).toHaveAccessibleDescription("Confirm content responsibility to enable conversion.");
    expect(screen.getByText("This confirmation is required before starting the conversion.")).toBeInTheDocument();

    await user.click(screen.getByLabelText("I confirm that I have the right to use and convert this file."));

    expect(createButton).toBeEnabled();
  });
});
