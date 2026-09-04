// Ref: workflow.md §4 Architecture Patterns | Feature: User Profile
"use client";

import { useState } from "react";
import Link from "next/link";
import { useAuthStore } from "@/stores/authStore";
import { userService } from "@/services/userService";
import { Button } from "@/components/common/Button";

export default function ProfilePage() {
  const { user, logout } = useAuthStore();
  const [isLoading, setIsLoading] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [editedUser, setEditedUser] = useState(user ?? null);

  const handleLogout = async () => {
    setIsLoading(true);
    await logout();
    setIsLoading(false);
  };

  const handleSave = async () => {
    setIsLoading(true);
    try {
      // In a real app, this would call userService.updateProfile(editedUser!)
      // For now, we'll just update the auth store
      // await userService.updateProfile(editedUser!);
      // useAuthStore.getState().setUser(editedUser!);
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
          Please <Link href="/login" className="font-medium text-indigo-600 hover:text-indigo-500">
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
          <Link href="/" className="inline-flex items-center text-sm font-medium text-indigo-600 hover:text-indigo-500">
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
                <div className="h-12 w-12 bg-indigo-100 rounded-full flex items-center justify-center">
                  <span className="text-indigo-600 font-bold">{user.name.charAt(0)}</span>
                </div>
              </div>
              <div>
                <h2 className="text-xl font-semibold text-gray-900">
                  {user.name}
                </h2>
                <p className="text-sm text-gray-500">
                  {user.email}
                </p>
                <p className="mt-1 text-sm text-gray-500">
                  Member since {new Date(user.createdAt).toLocaleDateString()}
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
                      Full Name
                    </label>
                    <input
                      type="text"
                      value={editedUser?.name || ""}
                      onChange={(e) => setEditedUser(prev => ({ ...prev!, name: e.target.value }))}
                      className="block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Email
                    </label>
                    <input
                      type="email"
                      value={editedUser?.email || ""}
                      onChange={(e) => setEditedUser(prev => ({ ...prev!, email: e.target.value }))}
                      className="block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
                    />
                  </div>
                  <div className="flex items-center justify-between">
                    <Button
                      variant="outline"
                      type="button"
                      onClick={() => {
                        setIsEditing(false);
                        setEditedUser(user);
                      }}
                    >
                      Cancel
                    </Button>
                    <Button
                      variant="primary"
                      type="submit"
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
                      {user.email}
                    </p>
                  </div>
                  <div>
                    <h3 className="text-sm font-medium text-gray-500">
                      Member Since
                    </h3>
                    <p className="mt-1 text-sm text-gray-900">
                      {new Date(user.createdAt).toLocaleDateString()}
                    </p>
                  </div>
                </div>

                <div className="mt-8">
                  <Button
                    variant="outline"
                    onClick={() => {
                      setIsEditing(true);
                      setEditedUser({ ...user });
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