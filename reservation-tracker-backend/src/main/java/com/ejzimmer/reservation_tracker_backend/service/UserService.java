package com.ejzimmer.reservation_tracker_backend.service;

import com.ejzimmer.reservation_tracker_backend.dto.UserDTO;

import java.util.List;

public interface UserService {
    List<UserDTO> getAllUserDTOs();
}
