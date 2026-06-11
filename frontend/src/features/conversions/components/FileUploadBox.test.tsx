import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { FileUploadBox } from "./FileUploadBox";

describe("FileUploadBox", () => {
  it("keeps the file input accessible", () => {
    render(<FileUploadBox file={null} onFileChange={vi.fn()} />);

    expect(screen.getByLabelText("Choose a DOCX file")).toHaveAttribute("type", "file");
  });

  it("accepts a dropped file", () => {
    const onFileChange = vi.fn();
    const file = new File(["content"], "document.docx", {
      type: "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    });

    render(<FileUploadBox file={null} onFileChange={onFileChange} />);

    fireEvent.drop(screen.getByText("Drag a DOCX here or select it from your device."), {
      dataTransfer: {
        files: [file],
      },
    });

    expect(onFileChange).toHaveBeenCalledWith(file);
  });
});
