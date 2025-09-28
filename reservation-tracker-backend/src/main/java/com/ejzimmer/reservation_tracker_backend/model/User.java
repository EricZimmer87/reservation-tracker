package com.ejzimmer.reservation_tracker_backend.model;

import jakarta.persistence.*;

@Entity
@Table(name = "users")
public class User {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long userId;

    @Column(nullable = false, unique = true)
    private String googleId;   // "sub" claim from Google

    @Column(nullable = false, unique = true, length = 100)
    private String email;

    @Column(name = "display_name", length = 100)
    private String displayName;  // from Google "name"

    @Column(columnDefinition = "TEXT")
    private String picture;      // profile pic URL

    @Column(nullable = false)
    private Boolean isAdmin = false;

    @Column(nullable = false)
    private Boolean isBanned = false;

    // Getters and Setters
    public Long getUserId() {
        return userId;
    }
    public void setUserId(Long userId) {
        this.userId = userId;
    }

    public String getGoogleId() {
        return googleId;
    }
    public void setGoogleId(String googleId) {
        this.googleId = googleId;
    }

    public String getEmail() {
        return email;
    }
    public void setEmail(String email) {
        this.email = email;
    }

    public String getDisplayName() {
        return displayName;
    }
    public void setDisplayName(String displayName) {
        this.displayName = displayName;
    }

    public String getPicture() {
        return picture;
    }
    public void setPicture(String picture) {
        this.picture = picture;
    }

    public Boolean getIsAdmin() {
        return isAdmin;
    }
    public void setIsAdmin(Boolean isAdmin) {
        this.isAdmin = isAdmin;
    }

    public Boolean getIsBanned() {
        return isBanned;
    }
    public void setIsBanned(Boolean isBanned) {
        this.isBanned = isBanned;
    }
}