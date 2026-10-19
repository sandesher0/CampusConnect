// Ref: workflow.md §4 Architecture Patterns | Feature: UI Components
import React, { useEffect, useRef } from "react";
import Button from "./Button";

interface ConfirmDialogProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => void;
  title: string;
  description: string;
  confirmLabel?: string;
  cancelLabel?: string;
  isDestructive?: boolean;
  isLoading?: boolean;
  triggerRef?: React.RefObject<HTMLElement | null>;
}

export default function ConfirmDialog({
  isOpen,
  onClose,
  onConfirm,
  title,
  description,
  confirmLabel = "Confirm",
  cancelLabel = "Cancel",
  isDestructive = false,
  isLoading = false,
  triggerRef,
}: ConfirmDialogProps) {
  const dialogRef = useRef<HTMLDialogElement>(null);

  useEffect(() => {
    const dialog = dialogRef.current;
    if (!dialog) return;

    if (isOpen) {
      if (!dialog.open) {
        dialog.showModal();
      }
    } else {
      if (dialog.open) {
        dialog.close();
      }
      triggerRef?.current?.focus();
    }
  }, [isOpen, triggerRef]);

  // Handle backdrop click
  const handleBackdropClick = (e: React.MouseEvent<HTMLDialogElement>) => {
    if (e.target === dialogRef.current && !isLoading) {
      onClose();
    }
  };

  return (
    <dialog
      ref={dialogRef}
      onCancel={(e) => {
        e.preventDefault();
        if (!isLoading) onClose();
      }}
      onClick={handleBackdropClick}
      className="m-auto backdrop:bg-ink/40 backdrop:backdrop-blur-sm rounded-[24px] border border-border-card bg-surface p-6 sm:p-8 max-w-md w-[92vw] shadow-[0_18px_40px_rgba(77,52,200,0.16)] animate-rise focus-visible:outline-none"
    >
      <div className="flex flex-col gap-3">
        <h3 className="text-xl font-[800] text-ink">{title}</h3>
        <p className="text-sm text-muted leading-relaxed">{description}</p>

        <div className="mt-6 flex flex-col-reverse sm:flex-row items-center justify-end gap-3 pt-2">
          <Button
            type="button"
            variant="outline"
            onClick={onClose}
            disabled={isLoading}
            className="w-full sm:w-auto"
          >
            {cancelLabel}
          </Button>

          <Button
            type="button"
            variant={isDestructive ? "destructive" : "primary"}
            onClick={onConfirm}
            loading={isLoading}
            className="w-full sm:w-auto"
          >
            {confirmLabel}
          </Button>
        </div>
      </div>
    </dialog>
  );
}
