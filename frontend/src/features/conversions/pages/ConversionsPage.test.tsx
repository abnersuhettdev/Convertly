import { screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { renderWithAppProviders } from "../../../test/test-utils";
import { ConversionsPage } from "./ConversionsPage";

const useConversionsMock = vi.fn();

vi.mock("../hooks/useConversions", () => ({
  useConversions: () => useConversionsMock(),
}));

describe("ConversionsPage", () => {
  it("renders a prominent new conversion link", () => {
    useConversionsMock.mockReturnValue({
      data: {
        items: [],
        page: 1,
        totalItems: 0,
        totalPages: 0,
      },
      error: null,
      isError: false,
      isLoading: false,
    });

    renderWithAppProviders(<ConversionsPage />);

    expect(screen.getByRole("link", { name: "New conversion" })).toHaveAttribute("href", "/conversions/new");
  });

  it("renders the central loading state while conversions are loading", () => {
    useConversionsMock.mockReturnValue({
      data: undefined,
      error: null,
      isError: false,
      isLoading: true,
    });

    renderWithAppProviders(<ConversionsPage />);

    expect(screen.getByRole("status")).toHaveTextContent("Loading conversions...");
    expect(screen.queryByText("No conversions found")).not.toBeInTheDocument();
  });
});
