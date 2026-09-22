// Ref: workflow.md §4 Architecture Patterns | Feature: User Profile
"use client";

import { useState } from "react";
import Link from "next/link";
import { useAuthStore } from "@/stores/authStore";
import { userService } from "@/services/userService";
import Button from "@/components/common/Button";

export default function ProfilePage() {
  const { user, logout } = useAuthStore();
  const [isLoading, setIsLoading] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [editedFirstName, setEditedFirstName] = useState(user?.user.firstName ?? "");
  const [editedLastName, setEditedLastName] = useState(user?.user.lastName ?? "");
  const [editedEmail, setEditedEmail] = useState(user?.user.email ?? "");

  const handleLogout = async () => {
    setIsLoading(true);
    await logout();
    setIsLoading(false);
  };

  const handleSave = async () => {
    setIsLoading(true);
    try {
      // In a real app, this would call userService.updateUser
      // await userService.updateUser(user!.id, {
      //   firstName: editedFirstName,
      //   lastName: editedLastName,
      //   email: editedEmail,
      // });
      // For now, we'll just update the auth store
      const updatedUser = {
        ...user!,
        firstName: editedFirstName,
        lastName: editedLastName,
        email: editedEmail,
      };
      // useAuthStore.getState().setUser(updatedUser);
      setIsEditing(false);
    } catch (error) {
      console.error("Failed to save profile:", error);
    } finally {
      setIsLoading(false);
    }
  };

  if (!user) {
    return (
      <div className="min-h-[calc(100vh-160px)] flex flex-col items-center justify-center py-12">
        <p className="text-gray-500">
          Please <Link href="/login" className="font-medium text-gray-900 hover:text-gray-600">
            sign in
          </Link> to view your profile.
        </p>
      </div>
    );
  }

  return (
    <div className="min-h-[calc(100vh-160px)] py-12">
      <div className="mx-auto max-w-2xl px-4 sm:px-6 lg:px-8">
        <div className="mb-8">
          <Link href="/" className="inline-flex items-center text-sm font-medium text-gray-900 hover:text-gray-600">
            ← Back to Home
          </Link>
          <h1 className="mt-4 text-2xl font-bold text-gray-900">
            My Profile
          </h1>
        </div>

        <div className="bg-white rounded-lg border border-gray-200 shadow-sm">
          <div className="p-6">
            <div className="flex items-center space-x-6">
              <div className="flex-shrink-0">
                <div className="h-12 w-12 bg-gray-100 rounded-full flex items-center justify-center">
                  <span className="text-gray-900 font-bold">
                    {(user.user.firstName ?? "")?.charAt(0) ?? ""}{(user.user.lastName ?? "")?.charAt(0) ?? ""}
                  </span>
                </div>
              </div>
              <div>
                <h2 className="text-xl font-semibold text-gray-900">
                  {user.user.firstName} {user.user.lastName}
                </h2>
                <p className="text-sm text-gray-500">
                  {user.user.email}
                </p>
                <p className="mt-1 text-sm text-gray-500">
                  Member since {new Date(user.accountVerifiedAt || "").toLocaleDateString()}
                </p>
              </div>
            </div>

            {isEditing ? (
              <div className="mt-6">
                <form className="space-y-4" onSubmit={(e) => {
                  e.preventDefault();
                  handleSave();
                }}>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      First Name
                    </label>
                    <input
                      type="text"
                      value={editedFirstName}
                      onChange={(e) => setEditedFirstName(e.target.value)}
                      className="block w-full rounded-md border-gray-300 shadow-sm focus:border-gray-900 focus:ring-gray-900"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Last Name
                    </label>
                    <input
                      type="text"
                      value={editedLastName}
                      onChange={(e) => setEditedLastName(e.target.value)}
                      className="block w-full rounded-md border-gray-300 shadow-sm focus:border-gray-900 focus:ring-gray-900"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Email
                    </label>
                    <input
                      type="email"
                      value={editedEmail}
                      onChange={(e) => setEditedEmail(e.target.value)}
                      className="block w-full rounded-md border-gray-300 shadow-sm focus:border-gray-900 focus:ring-gray-900"
                    />
                  </div>
                  <div className="flex items-center justify-between">
                    <Button
                      variant="outline"
                      onClick={() => {
                        setIsEditing(false);
                        // Reset edited fields to current user
                        setEditedFirstName(user.user.firstName);
                        setEditedLastName(user.user.lastName);
                        setEditedEmail(user.user.email);
                      }}
                    >
                      Cancel
                    </Button>
                    <Button
                      variant="primary"
                      onClick={handleSave}
                      disabled={isLoading}
                    >
                      {isLoading ? "Saving..." : "Save Changes"}
                    </Button>
                  </div>
                </form>
              </div>
            ) : (
              <>
                <div className="mt-6 grid grid-cols-1 gap-4 sm:grid-cols-2">
                  <div>
                    <h3 className="text-sm font-medium text-gray-500">
                      Email
                    </h3>
                    <p className="mt-1 text-sm text-gray-900">
                      {user.user.email}
                    </p>
                  </div>
                  <div>
                    <h3 className="text-sm font-medium text-gray-500">
                      Member Since
                    </h3>
                    <p className="mt-1 text-sm text-gray-900">
                      {new Date(user.accountVerifiedAt || "").toLocaleDateString()}
                    </p>
                  </div>
                </div>

                <div className="mt-8 flex flex-wrap gap-4">
                  <Button
                    variant="outline"
                    onClick={() => {
                      setIsEditing(true);
                      setEditedFirstName(user.user.firstName);
                      setEditedLastName(user.user.lastName);
                      setEditedEmail(user.user.email);
                    }}
                  >
                    Edit Profile
                  </Button>
                  <Button
                    variant="destructive"
                    onClick={handleLogout}
                    disabled={isLoading}
                  >
                    {isLoading ? "Logging out..." : "Log Out"}
                  </Button>
                </div>
              </>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}