package com.ejzimmer.reservation_tracker_backend.controller;

import com.ejzimmer.reservation_tracker_backend.service.UserService;
import com.ejzimmer.reservation_tracker_backend.dto.UserDTO;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.security.oauth2.core.user.OAuth2User;
import org.springframework.web.bind.annotation.*;

import java.util.Collections;
import java.util.List;
import java.util.Map;

@RestController
@RequestMapping(path="/api/users")
public class UserController {
    private final UserService userService;

    public UserController(UserService userService) {
        this.userService = userService;
    }

    // Get all UserDTOs
    @GetMapping
    public List<UserDTO> getAllUserDTOs() {
        return userService.getAllUserDTOs();
    }

    // Get user info
    @GetMapping("/me")
    public ResponseEntity<UserDTO> getCurrentUser(@AuthenticationPrincipal OAuth2User oauthUser) {
        try {
            return ResponseEntity.ok(userService.getCurrentUser(oauthUser));
        } catch (RuntimeException e) {
            return ResponseEntity.status(HttpStatus.UNAUTHORIZED).build();
        }
    }
}
