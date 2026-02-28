package com.pacman.entity;

import jakarta.persistence.*;
import lombok.Data;

import java.time.LocalDateTime;

@Data
@Entity
@Table(name = "users")
public class User {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(name = "username", nullable = false, unique = true, length = 50)
    private String username;

    @Column(name = "account", nullable = false, unique = true, length = 50)
    private String account;

    @Column(name = "password", nullable = false, length = 255)
    private String password;

    @Column(name = "security_question", nullable = false, length = 255)
    private String securityQuestion;

    @Column(name = "security_answer", nullable = false, length = 255)
    private String securityAnswer;

    @Column(name = "created_at", nullable = false, updatable = false)
    private LocalDateTime createdAt;

    @PrePersist
    protected void onCreate() {
        createdAt = LocalDateTime.now();
    }
}
