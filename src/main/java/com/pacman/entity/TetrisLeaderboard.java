package com.pacman.entity;

import jakarta.persistence.*;
import java.time.LocalDateTime;

@Entity
@Table(name = "tetris_leaderboard")
public class TetrisLeaderboard {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;
    
    private String username;
    private Integer score;
    private Integer lines;
    private LocalDateTime playedAt;
    
    public TetrisLeaderboard() {}
    
    public TetrisLeaderboard(String username, Integer score, Integer lines) {
        this.username = username;
        this.score = score;
        this.lines = lines;
        this.playedAt = LocalDateTime.now();
    }
    
    public Long getId() { return id; }
    public void setId(Long id) { this.id = id; }
    public String getUsername() { return username; }
    public void setUsername(String username) { this.username = username; }
    public Integer getScore() { return score; }
    public void setScore(Integer score) { this.score = score; }
    public Integer getLines() { return lines; }
    public void setLines(Integer lines) { this.lines = lines; }
    public LocalDateTime getPlayedAt() { return playedAt; }
    public void setPlayedAt(LocalDateTime playedAt) { this.playedAt = playedAt; }
}
