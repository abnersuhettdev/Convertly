import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { ConversionStatusBadge } from "./ConversionStatusBadge";

describe("ConversionStatusBadge", () => {
  it("renders the conversion status", () => {
    render(<ConversionStatusBadge status="Completed" />);

    expect(screen.getByText("Completed")).toBeInTheDocument();
  });
});
