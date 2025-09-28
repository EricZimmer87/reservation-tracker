package com.ejzimmer.reservation_tracker_backend.repository;

import org.springframework.data.jpa.repository.JpaRepository;
import com.ejzimmer.reservation_tracker_backend.model.User;

import java.util.Optional;

public interface UserRepository extends JpaRepository<User, Long> {
    Optional<User> findByEmail(String email);
}
