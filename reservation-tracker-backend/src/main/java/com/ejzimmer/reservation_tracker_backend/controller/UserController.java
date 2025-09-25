package com.ejzimmer.reservation_tracker_backend.controller;

import com.ejzimmer.reservation_tracker_backend.service.UserService;
import com.ejzimmer.reservation_tracker_backend.dto.UserDTO;
import org.springframework.web.bind.annotation.*;

import java.util.List;

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

}
